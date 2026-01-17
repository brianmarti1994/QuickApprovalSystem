using Domain.Audit;
using Domain.Requests;
using Domain.Users;
using Domain.Workflows;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstractions
{
    public interface IAppDbContext
    {
        DbSet<User> Users { get; }
        DbSet<Request> Requests { get; }
        DbSet<Workflow> Workflows { get; }
        DbSet<SystemLog> SystemLogs { get; }

        Task<int> SaveChangesAsync(CancellationToken ct);
    }
}
