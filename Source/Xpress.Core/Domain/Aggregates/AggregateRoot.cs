using System;
using System.Collections.Generic;
using System.Linq;
using Xpress.Core.Domain.Entities;
using Xpress.Core.Extensions;

namespace Xpress.Core.Domain.Aggregates
{
    public abstract class AggregateRoot<TAggregate, TIdentity> : Entity<TIdentity>, IAggregateRoot<TIdentity>
       where TAggregate : AggregateRoot<TAggregate, TIdentity>
       where TIdentity : IIdentity
    {
        private Lazy<IAggregateName> aggregateName = new Lazy<IAggregateName>(() => typeof(TAggregate).GetAggregateName());
        private Lazy<IReadOnlyDictionary<Type, Action<TAggregate, IAggregateEvent>>> applyMethods
            = new Lazy<IReadOnlyDictionary<Type, Action<TAggregate, IAggregateEvent>>>(() => typeof(TAggregate).GetAggregateEventApplyMethods<TAggregate, TIdentity, TAggregate>());

        private readonly Dictionary<Type, Action<object>> _eventHandlers = new Dictionary<Type, Action<object>>();
        private readonly List<IEventApplier<TAggregate, TIdentity>> _eventAppliers = new List<IEventApplier<TAggregate, TIdentity>>();

        protected virtual void ApplyEvent(IAggregateEvent<TAggregate, TIdentity> aggregateEvent)
        {
            if (aggregateEvent == null) throw new ArgumentNullException(nameof(aggregateEvent));

            var eventType = aggregateEvent.GetType();
            if (_eventHandlers.ContainsKey(eventType))
            {
                _eventHandlers[eventType](aggregateEvent);
            }
            else if (_eventAppliers.Any(ea => ea.Apply((TAggregate)this, aggregateEvent)))
            {
                // Already done
            }
            else
            {
                if (applyMethods.Value.TryGetValue(eventType, out var applyMethod))
                    applyMethod(this as TAggregate, aggregateEvent);
                else
                {
                    throw new NotImplementedException(
                        $"Aggregate '{aggregateName.Value}' does have an 'Apply' method that takes aggregate event '{eventType.PrettyPrint()}' as argument");
                }
            }
        }

        protected void Register<TAggregateEvent>(Action<TAggregateEvent> handler)
            where TAggregateEvent : IAggregateEvent<TAggregate, TIdentity>
        {
            if (handler == null) throw new ArgumentNullException(nameof(handler));

            var eventType = typeof(TAggregateEvent);
            if (_eventHandlers.ContainsKey(eventType))
                throw new ArgumentException($"There's already a event handler registered for the aggregate event '{eventType.PrettyPrint()}'");
            else
            {
                _eventHandlers[eventType] = e => handler((TAggregateEvent)e);
            }
        }

        protected void Register(IEventApplier<TAggregate, TIdentity> eventApplier)
        {
            if (eventApplier == null) throw new ArgumentNullException(nameof(eventApplier));

            _eventAppliers.Add(eventApplier);
        }

        public override string ToString()
        {
            return $"{GetType().PrettyPrint()}";
        }

    }
}
