using Domain.Requests;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Configurations
{
    public sealed class RequestConfig : IEntityTypeConfiguration<Request>
    {
        public void Configure(EntityTypeBuilder<Request> b)
        {
            b.ToTable("Requests");
            b.HasKey(x => x.Id);

            b.Property(x => x.Title).HasMaxLength(200).IsRequired();
            b.Property(x => x.Description).HasMaxLength(2000).IsRequired();
            b.Property(x => x.Amount).HasColumnType("decimal(18,2)");
            b.Property(x => x.Status).HasConversion<int>();

            b.OwnsMany(x => x.Decisions, db =>
            {
                db.ToTable("ApprovalDecisions");
                db.WithOwner().HasForeignKey(x => x.RequestId);
                db.HasKey(x => x.Id);
                db.Property(x => x.Comment).HasMaxLength(2000);
            });
        }
    }
}
