using Application.Abstractions;
using Domain.Audit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Logging
{
    public interface ISystemLogWriter
    {
        Task WriteAsync(string category, string action, string? detail, Guid? actorUserId, CancellationToken ct);
    }

    public sealed class SystemLogWriter : ISystemLogWriter
    {
        private readonly IAppDbContext _db;
        public SystemLogWriter(IAppDbContext db) => _db = db;

        public async Task WriteAsync(string category, string action, string? detail, Guid? actorUserId, CancellationToken ct)
        {
            _db.SystemLogs.Add(SystemLog.Create(category, action, detail, actorUserId));
            await _db.SaveChangesAsync(ct);
        }
    }
}
