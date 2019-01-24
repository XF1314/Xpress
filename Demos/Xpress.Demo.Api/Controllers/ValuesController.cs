using Exceptionless;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xpress.Core.BackgroundJobs;
using Xpress.Core.Caching;
using Xpress.Core.DependencyInjection;
using Xpress.Core.EventBus.Cap;
using Xpress.Core.EventBus.Local;
using Xpress.Core.Uow;
using Xpress.Demo.Api.Models;
using Xpress.Demo.Application;
using Xpress.Demo.Application.BackgroundJobs;
using Xpress.Demo.Application.LocalEventHandlers;
using Xpress.Demo.Core.Events;

namespace Xpress.Demo.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly ILogger<ValuesController> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IDemoService _demoService;
        private readonly IConfiguration _configuration;
        private readonly ILocalEventBus _localEventBus;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ICapEventPublisher _capEventPublisher;
        private readonly IBackgroundJobManager _backgroundJobManager;

        public ILogger<ValuesController> Logger { get; set; }

        public ValuesController(ILogger<ValuesController> logger, IDemoService demoService,
            IServiceScopeFactory serviceScopeFactory, IUnitOfWorkManager unitOfWorkManager, IBackgroundJobManager backgroundJobManager,
            IServiceProvider serviceProvider, IConfiguration configuration, ILocalEventBus localEventBus, ICapEventPublisher capEventPublisher)
        {
            _logger = logger;
            _demoService = demoService;
            _serviceProvider = serviceProvider;
            _unitOfWorkManager = unitOfWorkManager;
            _configuration = configuration;
            _localEventBus = localEventBus;
            _serviceScopeFactory = serviceScopeFactory;
            _capEventPublisher = capEventPublisher;
            _backgroundJobManager = backgroundJobManager;
            Logger = NullLogger<ValuesController>.Instance;//Controller 无法进行属性注入？？
        }


        // GET api/values
        [HttpGet, UnitOfWork(IsDisabled = false)]
        public ActionResult<IEnumerable<string>> Get()
        {
            //Logger.LogError("哈哈");
            //_logger.LogError("哈呵");
            _demoService.WriteMessage("呵呵");
            var pp = _configuration.GetValue<string>("XpressKey");
            var ss = _configuration.GetValue<int>("LocalParkId", 2);
            var kk = _configuration["ApolloConfig:AppId"];
            ExceptionlessClient.Default.CreateLog(nameof(ValuesController), "嘻嘻", Exceptionless.Logging.LogLevel.Fatal).Submit();

            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<ActionResult<string>> Get(int id)
        {
            var testCache = _serviceProvider.GetRequiredService<IDistributedCache<TestCacheItem>>();
            var cacheKey = Guid.NewGuid().ToString("N");
            var cacheItem = await testCache.GetOrAddAsync(cacheKey, async () => new TestCacheItem(name: id.ToString()));

            return cacheItem.Name;
        }

        // POST api/values
        [HttpPost]
        public async Task Post([FromBody] string value)
        {
            //var ss = typeof(TestLocalEventHandler).IsAssignableFrom(typeof(ILocalEventHandler));
            //var dd = typeof(TestLocalEventHandler).IsAssignableFrom(typeof(ITransientDependency));
            //var sd = typeof(TestLocalEventHandler).IsAssignableFrom(typeof(ILocalEventHandler<TestLocalEvent>));
            //await _localEventBus.PublishAsync(new TestLocalEvent { Message = value });
            //await _capEventPublisher.PublishAsync(new TestCapEvent { Message = value });
            await _backgroundJobManager.EnqueueAsync(new TestBackgroundEventArgs { Message = value });
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
