using Exceptionless;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xpress.Core.DependencyInjection;
using Xpress.Core.EventBus.Local;
using Xpress.Demo.Application.CapSubscribers;
using Xpress.Demo.Core.Events;

namespace Xpress.Demo.Application.LocalEventHandlers
{
    public class TestLocalEventHandler : ILocalEventHandler<TestLocalEvent>, ITransientDependency
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<TestLocalEventHandler> _logger;

        public TestLocalEventHandler(IServiceProvider serviceProvider, ILogger<TestLocalEventHandler> logger)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public async Task HandleEventAsync(TestLocalEvent @event)
        {
            _logger.LogCritical(@event.Message);
            //ExceptionlessClient.Default.CreateLog(nameof(TestLocalEventHandler), @event.Message, Exceptionless.Logging.LogLevel.Fatal).Submit();

            await Task.CompletedTask;
        }
    }
}
