using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Xpress.Core.Collections;
using Xpress.Core.DependencyInjection;
using Xpress.Core.EventBus.Local;
using Xpress.Core.Exceptions;
using Xpress.Core.Extensions;
using Xpress.Core.Utils;

namespace Xpress.Core.EventBus.Local
{
    public class LocalEventBus : ILocalEventBus, ISingletonDependency
    {
        /// <summary>
        /// Reference to the Logger.
        /// </summary>
        public ILogger<LocalEventBus> Logger { get; set; }

        protected LocalEventBusOptions Options { get; }

        protected IServiceScopeFactory ServiceScopeFactory { get; }

        protected ConcurrentDictionary<Type, List<IEventHandlerFactory>> HandlerFactories { get; }

        public LocalEventBus(IOptions<LocalEventBusOptions> options, IServiceScopeFactory serviceScopeFactory)
        {
            Options = options.Value;
            ServiceScopeFactory = serviceScopeFactory;
            Logger = NullLogger<LocalEventBus>.Instance;
            HandlerFactories = new ConcurrentDictionary<Type, List<IEventHandlerFactory>>();

            SubscribeHandlers();
        }

        #region Subscribe
        /// <inheritdoc/>
        public virtual IDisposable Subscribe<TEvent>(Func<TEvent, Task> action)
            where TEvent : IEvent
        {
            return Subscribe(typeof(TEvent), new ActionEventHandler<TEvent>(action));
        }

        /// <inheritdoc/>
        public virtual IDisposable Subscribe<TEvent, THandler>()
            where TEvent : IEvent
            where THandler : ILocalEventHandler, new()
        {
            return Subscribe(typeof(TEvent), new TransientEventHandlerFactory<THandler>());
        }

        /// <inheritdoc/>
        public virtual IDisposable Subscribe(Type eventType, ILocalEventHandler handler)
        {
            return Subscribe(eventType, new SingleInstanceHandlerFactory(handler));
        }

        /// <inheritdoc/>
        public virtual IDisposable Subscribe<TEvent>(IEventHandlerFactory factory)
            where TEvent : IEvent
        {
            return Subscribe(typeof(TEvent), factory);
        }

        /// <inheritdoc/>
        /// <inheritdoc/>
        public IDisposable Subscribe(Type eventType, IEventHandlerFactory factory)
        {
            GetOrCreateHandlerFactories(eventType)
                .Locking(factories => factories.Add(factory));

            return new EventHandlerFactoryUnregistrar(this, eventType, factory);
        }

        protected virtual void SubscribeHandlers()
        {
            foreach (var handler in Options.Handlers)
            {
                var interfaces = handler.GetInterfaces();
                foreach (var @interface in interfaces)
                {
                    var genericArgs = @interface.GetGenericArguments();
                    if (!typeof(ILocalEventHandler).GetTypeInfo().IsAssignableFrom(@interface) || genericArgs.Length != 1)
                        continue;
                    else
                    {
                        Subscribe(genericArgs[0], new IocEventHandlerFactory(ServiceScopeFactory, handler));
                    }
                }
            }
        }
        #endregion

        #region UnSubscribe
        /// <inheritdoc/>
        public void Unsubscribe<TEvent>(Func<TEvent, Task> action)
            where TEvent : IEvent
        {
            Check.NotNull(action, nameof(action));

            GetOrCreateHandlerFactories(typeof(TEvent))
                .Locking(factories =>
                {
                    factories.RemoveAll(
                        factory =>
                        {
                            var singleInstanceFactory = factory as SingleInstanceHandlerFactory;
                            if (singleInstanceFactory == null)
                            {
                                return false;
                            }

                            var actionHandler = singleInstanceFactory.HandlerInstance as ActionEventHandler<TEvent>;
                            if (actionHandler == null)
                            {
                                return false;
                            }

                            return actionHandler.Action == action;
                        });
                });
        }

        /// <inheritdoc/>
        public virtual void Unsubscribe<TEvent>(ILocalEventHandler<TEvent> handler)
            where TEvent : IEvent
        {
            Unsubscribe(typeof(TEvent), handler);
        }

        public void Unsubscribe(Type eventType, ILocalEventHandler handler)
        {
            GetOrCreateHandlerFactories(eventType)
                .Locking(factories =>
                {
                    factories.RemoveAll(
                        factory =>
                            factory is SingleInstanceHandlerFactory &&
                            (factory as SingleInstanceHandlerFactory).HandlerInstance == handler
                    );
                });
        }

        /// <inheritdoc/>
        public virtual void Unsubscribe<TEvent>(IEventHandlerFactory factory)
            where TEvent : IEvent
        {
            Unsubscribe(typeof(TEvent), factory);
        }

        /// <inheritdoc/>
        public void Unsubscribe(Type eventType, IEventHandlerFactory factory)
        {
            GetOrCreateHandlerFactories(eventType).Locking(factories => factories.Remove(factory));
        }

        /// <inheritdoc/>
        public virtual void UnsubscribeAll<TEvent>()
            where TEvent : IEvent
        {
            UnsubscribeAll(typeof(TEvent));
        }

        /// <inheritdoc/>
        public void UnsubscribeAll(Type eventType)
        {
            GetOrCreateHandlerFactories(eventType).Locking(factories => factories.Clear());
        }
        #endregion

        #region Publish
        /// <inheritdoc/>
        public virtual Task PublishAsync<TEvent>(TEvent eventData)
            where TEvent : IEvent
        {
            return PublishAsync(typeof(TEvent), eventData);
        }

        public async Task PublishAsync(Type eventType, IEvent eventData)
        {
            var exceptions = new List<Exception>();

            await TriggerHandlersAsync(eventType, eventData, exceptions);

            if (exceptions.Any())
            {
                if (exceptions.Count == 1)
                {
                    exceptions[0].ReThrow();
                }

                throw new AggregateException("More than one error has occurred while triggering the event: " + eventType, exceptions);
            }
        }
        #endregion

        #region TriggerHandler
        public virtual async Task TriggerHandlersAsync(Type eventType, IEvent eventData)
        {
            var exceptions = new List<Exception>();

            await TriggerHandlersAsync(eventType, eventData, exceptions);

            if (exceptions.Any())
            {
                if (exceptions.Count == 1)
                {
                    exceptions[0].ReThrow();
                }

                throw new AggregateException("More than one error has occurred while triggering the event: " + eventType, exceptions);
            }
        }

        protected virtual async Task TriggerHandlersAsync(Type eventType, IEvent eventData, List<Exception> exceptions)
        {
            await new SynchronizationContextRemover();

            foreach (var handlerFactories in GetHandlerFactories(eventType))
            {
                foreach (var handlerFactory in handlerFactories.EventHandlerFactories.ToArray()) //TODO: ToArray should not be needed!
                {
                    await TriggerHandlerAsync(handlerFactory, handlerFactories.EventType, eventData, exceptions);
                }
            }

            //Implements generic argument inheritance. See IEventDataWithInheritableGenericArgument
            if (eventType.GetTypeInfo().IsGenericType &&
                eventType.GetGenericArguments().Length == 1 &&
                typeof(IEventDataWithInheritableGenericArgument).IsAssignableFrom(eventType))
            {
                var genericArg = eventType.GetGenericArguments()[0];
                var baseArg = genericArg.GetTypeInfo().BaseType;
                if (baseArg != null)
                {
                    var baseEventType = eventType.GetGenericTypeDefinition().MakeGenericType(baseArg);
                    var constructorArgs = ((IEventDataWithInheritableGenericArgument)eventData).GetConstructorArgs();
                    var baseEventData = (IEvent)Activator.CreateInstance(baseEventType, constructorArgs);
                    await PublishAsync(baseEventType, baseEventData);
                }
            }
        }

        protected virtual async Task TriggerHandlerAsync(IEventHandlerFactory asyncHandlerFactory, Type eventType, IEvent eventData, List<Exception> exceptions)
        {
            using (var eventHandlerWrapper = asyncHandlerFactory.GetHandler())
            {
                try
                {
                    var handlerType = eventHandlerWrapper.EventHandler.GetType();

                    if (ReflectionHelper.IsAssignableToGenericType(handlerType, typeof(ILocalEventHandler<>)))
                    {
                        var method = typeof(ILocalEventHandler<>)
                            .MakeGenericType(eventType)
                            .GetMethod(
                                nameof(ILocalEventHandler<IEvent>.HandleEventAsync),
                                new[] { eventType }
                            );

                        await (Task)method.Invoke(eventHandlerWrapper.EventHandler, new[] { eventData });
                    }
                    else
                    {
                        throw new XpressException("The object instance is not an event handler. Object type: " + handlerType.AssemblyQualifiedName);
                    }
                }
                catch (TargetInvocationException ex)
                {
                    exceptions.Add(ex.InnerException);
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }
        }
        #endregion

        protected class EventTypeWithEventHandlerFactories
        {
            public Type EventType { get; }

            public List<IEventHandlerFactory> EventHandlerFactories { get; }

            public EventTypeWithEventHandlerFactories(Type eventType, List<IEventHandlerFactory> eventHandlerFactories)
            {
                EventType = eventType;
                EventHandlerFactories = eventHandlerFactories;
            }
        }

        // Reference from
        // https://blogs.msdn.microsoft.com/benwilli/2017/02/09/an-alternative-to-configureawaitfalse-everywhere/
        protected struct SynchronizationContextRemover : INotifyCompletion
        {
            public bool IsCompleted
            {
                get { return SynchronizationContext.Current == null; }
            }

            public void OnCompleted(Action continuation)
            {
                var prevContext = SynchronizationContext.Current;
                try
                {
                    SynchronizationContext.SetSynchronizationContext(null);
                    continuation();
                }
                finally
                {
                    SynchronizationContext.SetSynchronizationContext(prevContext);
                }
            }

            public SynchronizationContextRemover GetAwaiter()
            {
                return this;
            }

            public void GetResult()
            {
            }
        }

        protected IEnumerable<EventTypeWithEventHandlerFactories> GetHandlerFactories(Type eventType)
        {
            var handlerFactoryList = new List<EventTypeWithEventHandlerFactories>();

            foreach (var handlerFactory in HandlerFactories.Where(hf => ShouldTriggerEventForHandler(eventType, hf.Key)))
            {
                handlerFactoryList.Add(new EventTypeWithEventHandlerFactories(handlerFactory.Key, handlerFactory.Value));
            }

            return handlerFactoryList.ToArray();
        }

        private List<IEventHandlerFactory> GetOrCreateHandlerFactories(Type eventType)
        {
            return HandlerFactories.GetOrAdd(eventType, (type) => new List<IEventHandlerFactory>());
        }

        private static bool ShouldTriggerEventForHandler(Type targeTEventType, Type handlerEventType)
        {
            //Should trigger same type
            if (handlerEventType == targeTEventType)
            {
                return true;
            }

            //Should trigger for inherited types
            if (handlerEventType.IsAssignableFrom(targeTEventType))
            {
                return true;
            }

            return false;
        }
    }
}
