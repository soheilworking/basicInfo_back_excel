using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using ORG.BasicInfo.Domain.FormAggregate;

namespace ORG.BasicInfo.Data.Configurations.FormRaw
{
    public class FormRawLogSysConfiguration : IEntityTypeConfiguration<FormRawLogSys>
    {
        public void Configure(EntityTypeBuilder<FormRawLogSys> builder)
        {
            builder
        .HasKey(log => log.Id);

            builder
                .Property(log => log.Id)
                .IsRequired()
                .ValueGeneratedNever();

            builder
              .HasIndex(log => log.Id)
              .IsUnique();
            builder
                .Property(log => log.Ip)
                .IsRequired();
            builder
             .Property(log => log.Description)
             .IsUnicode(true)
             .IsRequired()
             .HasMaxLength(150);

            builder
                     .Property(log => log.IdUser).IsRequired();


            builder
                     .Property(log => log.IdUser).IsRequired();

            builder
              .Property(log => log.IdForm).IsRequired();
        }
    }
}
