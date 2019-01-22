using DotNetCore.CAP;
using Exceptionless;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xpress.Core.DependencyInjection;
using Xpress.Core.EventBus.Cap;
using Xpress.Core.Uow;
using Xpress.Demo.Core.Events;

namespace Xpress.Demo.Application.CapSubscribers
{
    public class CapEventSubscriber : ICapEventSubscriber
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<CapEventSubscriber> _logger;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public CapEventSubscriber(IServiceProvider serviceProvider, ILogger<CapEventSubscriber> logger ,IUnitOfWorkManager unitOfWorkManager)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _unitOfWorkManager = unitOfWorkManager;
        }

        [CapSubscribe("TestCap")]
        public async Task TestCap(TestCapEvent testCapEvent)
        {
            _logger.LogInformation(testCapEvent.Message);
            ExceptionlessClient.Default.CreateLog(nameof(CapEventSubscriber), testCapEvent.Message, Exceptionless.Logging.LogLevel.Fatal).Submit();

            await Task.CompletedTask;
        }
    }
}
