using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using ORG.BasicInfo.Domain.FormUserAggregate;

namespace ORG.BasicInfo.Data.Configurations.FormUser
{
    public class FormUserLogSysConfiguration : IEntityTypeConfiguration<FormUserLogSys>
    {
        public void Configure(EntityTypeBuilder<FormUserLogSys> builder)
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
             .Property(log => log.Description)
             .IsUnicode(true)
             .IsRequired()
             .HasMaxLength(150);

            builder
         .Property(log => log.Ip)
         .IsUnicode(true)
         .IsRequired()
         .HasMaxLength(20);


            builder
                     .Property(log => log.IdUser).IsRequired();



            builder
            .Property(log => log.StateAction).IsRequired();


            builder
                .Property(log => log.Timestamp).IsRequired();

            builder
              .Property(log => log.IdFormUser).IsRequired();

            builder
           .Property(log => log.IdUserRead).IsRequired();
        }
    }
}
