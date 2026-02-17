using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ORG.BasicInfo.Domain.UserAggregate;

namespace ORG.BasicInfo.Data.Configurations
{
    public class PermissionFundConfiguration : IEntityTypeConfiguration<PermissionFund>
    {
        public void Configure(EntityTypeBuilder<PermissionFund> builder)
        {
            builder
                .HasKey(user => user.Id);

            builder
                .Property(user => user.Id)
                .IsRequired()
                .ValueGeneratedNever();

            builder
                .Property(user => user.IdUser)
                .IsRequired()
                .HasMaxLength(500);

            builder
             .HasIndex(log => log.IdUser);

            builder
                .Property(user => user.IdFund)
                .IsRequired()
                .HasMaxLength(500);
        }

    }
}
