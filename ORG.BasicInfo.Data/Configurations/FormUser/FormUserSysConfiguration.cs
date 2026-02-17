using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using ORG.BasicInfo.Domain.FormUserAggregate;

namespace ORG.BasicInfo.Data.Configurations.FormUser
{
    public class FormUserSysConfiguration : IEntityTypeConfiguration<FormUserSys>
    {
        public void Configure(EntityTypeBuilder<FormUserSys> builder)
        {

            builder
                    .HasKey(fileForm => fileForm.Id);

            builder
                .Property(fileForm => fileForm.Id)
                .IsRequired()
                .ValueGeneratedNever();

            builder
              .HasIndex(fileForm => fileForm.Id)
              .IsUnique();

            builder
             .Property(fileForm => fileForm.Description)
             .IsUnicode(true)
             .IsRequired()
             .HasMaxLength(2000);


            builder
              .Property(fileForm => fileForm.IdFormRaw)
              .IsRequired();

            builder
              .Property(fileForm => fileForm.IdUser)
              .IsRequired();

            builder
              .Property(fileForm => fileForm.LUserEdit)
              .IsRequired();

            builder
              .Property(fileForm => fileForm.TEdit)
              .IsRequired();

            builder
              .Property(fileForm => fileForm.State)
              .IsRequired();

            builder
            .Property(fileForm => fileForm.TCreate)
            .IsRequired();

            builder
              .Property(fileForm => fileForm.IdUserRead)
              .IsRequired();

            builder
             .Property(fileForm => fileForm.LUserCreate)
             .IsRequired();

            
        }
    }
}
