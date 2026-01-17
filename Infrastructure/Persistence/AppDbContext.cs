using Application.Abstractions;
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

namespace Infrastructure.Persistence
{
    public sealed class AppDbContext : DbContext, IAppDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Request> Requests => Set<Request>();
        public DbSet<Workflow> Workflows => Set<Workflow>();
        public DbSet<SystemLog> SystemLogs => Set<SystemLog>();

        protected override void OnModelCreating(ModelBuilder b)
        {
            b.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
            base.OnModelCreating(b);
        }
    }
}
