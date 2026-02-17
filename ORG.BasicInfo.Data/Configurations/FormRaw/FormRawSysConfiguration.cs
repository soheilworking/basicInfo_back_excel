using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ORG.BasicInfo.Domain.FormAggregate;

namespace ORG.BasicInfo.Data.Configurations.FormRaw;

public class FormRawSysConfiguration : IEntityTypeConfiguration<FormRawSys>
{
    public void Configure(EntityTypeBuilder<FormRawSys> builder)
    {
        builder.HasKey(customer => customer.Id);

        builder.Property(customer => customer.Id)
            .IsRequired()
            .ValueGeneratedNever();

        builder.HasIndex(customer => customer.Id).IsUnique();

        builder.HasMany(x => x.FilesRawOrgSys)
          .WithOne()
          .HasForeignKey(a => a.IdFormRaw);

        builder.Property(customer => customer.IdCode)
            .IsRequired()
            .ValueGeneratedNever();

        builder.HasIndex(customer => customer.IdCode).IsUnique();

        builder.Property(customer => customer.Title)
            .IsUnicode(true)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(customer => customer.Title);
      
        builder
         .Property(user => user.TCreate)
         .IsRequired()
         .ValueGeneratedNever();

        builder
         .Property(user => user.TEdit)
         .IsRequired()
         .ValueGeneratedNever();

        builder
         .Property(user => user.ExpireDate)
         .IsRequired()
         .ValueGeneratedNever();

        builder.HasIndex(customer => customer.TCreate);

        builder.HasIndex(customer => customer.TEdit);
        builder.HasIndex(customer => customer.ExpireDate);

        builder.Property(customer => customer.Description)
        .IsUnicode(true)
        .IsRequired()
        .HasMaxLength(600);

        builder.HasMany(f => f.UserFund)
       .WithOne() 
       .HasForeignKey(fu => fu.IdForm);

    }
}
