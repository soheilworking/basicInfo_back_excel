using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ORG.BasicInfo.Domain.FormAggregate;
using ORG.BasicInfo.Domain.FormUserAggregate;
using ORG.BasicInfo.Domain.UserAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORG.BasicInfo.Data.Configurations.FormUser
{
    public class FilesFormsSysConfiguration : IEntityTypeConfiguration<FilesFormsSys>
    {
        public void Configure(EntityTypeBuilder<FilesFormsSys> builder)
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
             .Property(fileForm => fileForm.Title)
             .IsUnicode(true)
             .IsRequired()
             .HasMaxLength(40);

            builder
          .Property(fileForm => fileForm.LUserCreate)
          .IsRequired();



            builder
          .Property(fileForm => fileForm.UploadDate)
          .IsRequired();



            builder
          .Property(fileForm => fileForm.KeyFile)
          .IsRequired();


            builder
            .Property(fileForm => fileForm.IdFormUser)
            .IsRequired();
        }
    }
}
