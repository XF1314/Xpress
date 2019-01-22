using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Com.Ctrip.Framework.Apollo;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using NLog.Web;

namespace Xpress.Demo.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //NLog.config||NLog.Exceptionless.config
            var logger = NLogBuilder.ConfigureNLog("NLog.Exceptionless.config").GetCurrentClassLogger();

            try
            {
                logger.Debug("initial application");
                CreateWebHostBuilder(args).Build().Run();
            }
            catch (Exception e)
            {
                logger.Fatal(e, "Stopped application by fatal exception");
                throw;
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                    .ConfigureAppConfiguration((hostingContext, configurationBuilder) =>
                    {
                        configurationBuilder
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json")
                        .AddApollo(configurationBuilder.Build().GetSection("ApolloConfig"))
                        .AddNamespace("ThemePark.Common")
                        .AddDefault();
                    })
                    .ConfigureLogging((context, builder) =>
                    {
                        builder.ClearProviders();
                        builder.AddNLog(new NLogProviderOptions
                        {
                            CaptureMessageProperties = true,
                            CaptureMessageTemplates = true
                        });
                    })
                    .UseStartup<Startup>();
                }
    }
}
