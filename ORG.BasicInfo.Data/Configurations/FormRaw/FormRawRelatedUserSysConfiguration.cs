using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ORG.BasicInfo.Domain.FormAggregate;


namespace ORG.BasicInfo.Data.Configurations.FormRaw;

public class FormRawRelatedUserSysConfiguration : IEntityTypeConfiguration<FormRawRelatedUserSys>
{
    public void Configure(EntityTypeBuilder<FormRawRelatedUserSys> builder)
    {

        builder
           .HasKey(customer => customer.Id);

        builder
            .Property(customer => customer.Id)
            .IsRequired()
            .ValueGeneratedNever();

        builder
          .HasIndex(customer => customer.Id)
          .IsUnique();

        builder
            .Property(customer => customer.IdUser)
            .IsRequired()
            .ValueGeneratedNever();

        builder
        .Property(customer => customer.IdForm)
        .IsRequired()
        .ValueGeneratedNever();

        builder.HasOne(fu => fu.User)
       .WithMany() 
       .HasForeignKey(fu => fu.IdUser);







    }
}