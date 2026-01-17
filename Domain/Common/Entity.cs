using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Common
{
    public abstract class Entity<TId>
    {
        public TId Id { get; protected set; } = default!;
    }

    public abstract class AggregateRoot<TId> : Entity<TId>
    {
        private readonly List<IDomainEvent> _events = new();
        public IReadOnlyCollection<IDomainEvent> Events => _events;

        protected void Raise(IDomainEvent @event) => _events.Add(@event);
        public void ClearEvents() => _events.Clear();
    }

    public interface IDomainEvent
    {
        DateTime OccurredAtUtc { get; }
    }
}
