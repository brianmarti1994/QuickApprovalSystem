using Domain.Workflows;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Configurations
{
    public sealed class WorkflowConfig : IEntityTypeConfiguration<Workflow>
    {
        public void Configure(EntityTypeBuilder<Workflow> b)
        {
            b.ToTable("Workflows");
            b.HasKey(x => x.Id);
            b.Property(x => x.Name).HasMaxLength(200).IsRequired();

            b.OwnsMany(x => x.Steps, sb =>
            {
                sb.ToTable("WorkflowSteps");
                sb.WithOwner().HasForeignKey(x => x.WorkflowId);
                sb.HasKey(x => x.Id);
                sb.Property(x => x.Label).HasMaxLength(200).IsRequired();
                sb.Property(x => x.ApproverRole).HasConversion<int>();
                sb.HasIndex(x => new { x.WorkflowId, x.Order }).IsUnique();
            });
        }
    }
}
