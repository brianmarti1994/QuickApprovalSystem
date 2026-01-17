using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Audit
{
    public sealed class SystemLog : Entity<long>
    {
        private SystemLog() { }

        public string Category { get; private set; } = default!;
        public string Action { get; private set; } = default!;
        public string? Detail { get; private set; }
        public Guid? ActorUserId { get; private set; }
        public DateTime AtUtc { get; private set; }

        public static SystemLog Create(string category, string action, string? detail, Guid? actorUserId)
            => new()
            {
                Category = category,
                Action = action,
                Detail = detail,
                ActorUserId = actorUserId,
                AtUtc = DateTime.UtcNow
            };
    }
}
