using Castle.Core.Logging;
using Exceptionless;
using Hangfire;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLog.Web;
using OrderServer;
using StackExchange.Redis;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.IO;
using TicketServer;
using VipCardServer;
using Xpress.AspNetCore;
using Xpress.AspNetCore.ExceptionHandling;
using Xpress.AspNetCore.ModelValidation;
using Xpress.AspNetCore.Uow;
using Xpress.Autofac;
using Xpress.AutoMapper;
using Xpress.CastleWindsor;
using Xpress.Core;
using Xpress.Core.Caching;
using Xpress.Core.EntityFramework;
using Xpress.Core.EventBus.Cap;
using Xpress.Core.EventBus.Local;
using Xpress.Demo.Application;
using Xpress.Demo.Application.CapSubscribers;
using Xpress.Demo.Core;
using Xpress.Demo.EntityFramework;
using HangfireRedisStorage = Hangfire.Redis.RedisStorage;
using HangfireDashboardOptions = Hangfire.DashboardOptions;
using Hangfire.Redis;
using Xpress.Demo.Api.Filters;
using Xpress.Demo.Application.RecurringJobs;
using Hangfire.RecurringJobExtensions;

namespace Xpress.Demo.Api
{

    public class Startup
    {
        public IConfiguration Configuration { get; private set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            //Distribute Cache
            var cacheRedisConnectionString = Configuration.GetConnectionString("CacheRedis");
            var cacheRedisConnection = ConnectionMultiplexer.Connect(cacheRedisConnectionString);
            services.AddSingleton<IConnectionMultiplexer>(cacheRedisConnection);
            services.AddDistributedRedisCache(x => x.Configuration = cacheRedisConnectionString);
            services.AddSingleton(typeof(IDistributedCache<>), typeof(DistributedCache<>));

            //RDBMS Storage
            services.AddDbContext<EfDbContextBase, DemoDbContext>(options => options.UseMySql(Configuration.GetConnectionString("Default")));

            //Hangfire Job
            var jobRedisConnectionString = Configuration.GetConnectionString("JobRedis");
            var jobRedisConnection = ConnectionMultiplexer.Connect(jobRedisConnectionString);
            services.AddHangfire(x =>
            {
                x.UseRedisStorage(jobRedisConnection); //Job Storage
                x.UseRecurringJob(typeof(TestRecurringJob));
                x.UseDashboardMetric(DashboardMetrics.ServerCount)// DashboardMetric
                    .UseDashboardMetric(DashboardMetrics.RecurringJobCount)
                    .UseDashboardMetric(DashboardMetrics.RetriesCount)
                    .UseDashboardMetric(DashboardMetrics.AwaitingCount)
                    .UseDashboardMetric(DashboardMetrics.EnqueuedAndQueueCount)
                    .UseDashboardMetric(DashboardMetrics.ScheduledCount)
                    .UseDashboardMetric(DashboardMetrics.ProcessingCount)
                    .UseDashboardMetric(DashboardMetrics.SucceededCount)
                    .UseDashboardMetric(DashboardMetrics.FailedCount)
                    .UseDashboardMetric(DashboardMetrics.DeletedCount);
                if (JobStorage.Current is HangfireRedisStorage redisStorage)
                {
                    x.UseDashboardMetric(
                        redisStorage.GetDashboardMetricFromRedisInfo("使用内存", RedisInfoKeys.used_memory_human));
                    x.UseDashboardMetric(
                        redisStorage.GetDashboardMetricFromRedisInfo("高峰内存", RedisInfoKeys.used_memory_peak_human));
                }

            });

            //Data Protection
            var applicationDiscriminator = Configuration.GetValue<string>("ApplicationDiscriminator");
            services.AddDataProtection(x => x.ApplicationDiscriminator = applicationDiscriminator).PersistKeysToRedis(cacheRedisConnection);

            //Swagger Api Document
            services.AddSwaggerGen(options =>
            {
                var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                options.SwaggerDoc("v1", new Info { Version = "v1", Title = "Xpress示例项目API" });
                options.IncludeXmlComments(Path.Combine(baseDirectory, $"Xpress.Demo.Application.xml"));
                options.IncludeXmlComments(Path.Combine(baseDirectory, $"Xpress.Demo.Api.xml"));
            });

            //LocalEventBus && Cap(最终一致性事件总线)
            services.Configure<LocalEventBusOptions>(x => { });
            services.AddTransient<ICapEventSubscriber, CapEventSubscriber>();
            services.AddCap(capOptions =>
            {
                capOptions.SucceedMessageExpiredAfter = 60 * 60 * 24 * 2; //成功的消息保存2天
                capOptions.UseMySql(mySqlOptions =>
                {
                    mySqlOptions.TableNamePrefix = "Cap.Xpress.Api";
                    mySqlOptions.ConnectionString = Configuration.GetConnectionString("Default");
                });
                capOptions.UseRabbitMQ(configure =>
                {
                    var rabbitMqConfig = Configuration.GetSection("RabbitMq");
                    rabbitMqConfig.Bind(configure);
                });
            });

            services.AddMvc(options =>
            {
                options.Filters.Add<UowActionFilter>();
                options.Filters.Add<XpressExceptionFilter>();
                options.Filters.Add<XpressModelValidationActionFilter>();
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddCors(options =>
            {
                var corsAllowedHosts = Configuration.GetValue<string>("CorsAllowedHosts")?.Split(new char[] { ';' });
                options.AddDefaultPolicy(p =>
                {
                    p.WithOrigins(corsAllowedHosts)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

            return services.AddXpress(options =>
             {
                 options.UowOptions.ConventionalUowSelectors.Add(x => x.IsSubclassOf(typeof(ControllerBase)));
                 options.UseAutofac();
                 options.AddAspNetCore();
                 options.AddOrderServer();
                 options.AddTicketServer();
                 options.AddVipCardServer();

                 options.AddDemoCore();
                 options.AddDemoApplication();
             });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();
            app.UseXpress(options =>
            {
                options.UseAutoMapper(mapConfiguration =>
                {

                });
            });

            app.UseMvc();
            app.UseSwagger();
            app.UseStaticFiles();
            app.UseExceptionless(Configuration);
            app.UseMiddleware<UnitOfWorkMiddleware>();//需要注意Middleware的位置
            app.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "Xpress示例项目API"));
            app.UseHangfireServer(new BackgroundJobServerOptions
            {
                WorkerCount = Math.Min(Environment.ProcessorCount * 5, 20),
                Queues = new string[] { "default" }
            });
            app.UseHangfireDashboard("/hangfire", new HangfireDashboardOptions()
            {
                DisplayStorageConnectionString = false,
                Authorization = new IDashboardAuthorizationFilter[]
                {
                    new JobAuthorizationFilter(env)
                }
            });
        }
    }
}
