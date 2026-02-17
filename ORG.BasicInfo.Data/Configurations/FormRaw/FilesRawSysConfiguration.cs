using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ORG.BasicInfo.Domain.FormAggregate;

namespace ORG.BasicInfo.Data.Configurations.FormRaw;

public class FilesRawSysConfiguration : IEntityTypeConfiguration<FilesRawSys>
{
    public void Configure(EntityTypeBuilder<FilesRawSys> builder)
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
            .Property(customer => customer.Title)
            .IsUnicode(true)
            .IsRequired()
            .HasMaxLength(50);

        builder
        .Property(customer => customer.KeyFile)
        .IsRequired()
        .HasMaxLength(200);


        builder
        .Property(customer => customer.UploadDate)
        .IsRequired();

        builder
          .HasIndex(customer => customer.UploadDate);


        builder
        .Property(customer => customer.LUserCreate)
        .IsRequired();
        
        builder
        .HasIndex(customer => customer.LUserCreate);



    }
}