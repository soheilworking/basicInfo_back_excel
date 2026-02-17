using ORG.BasicInfo.Domain.UserAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ORG.BasicInfo.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder
            .HasKey(user => user.Id);

        builder
            .Property(user => user.Id)
            .IsRequired()
            .ValueGeneratedNever();

        builder
            .Property(user => user.Name)
             .IsUnicode(true)
            .IsRequired()
            .HasMaxLength(40);

        builder
            .Property(user => user.LastName)
            .IsUnicode(true)
            .IsRequired()
            .HasMaxLength(40);

        builder
            .Property(user => user.Mobile)
            .IsRequired();

        builder
           .HasIndex(user => user.Mobile)
           .IsUnique();

        builder
            .Property(user => user.UserRole)
            .IsRequired()
            .ValueGeneratedNever();

        builder
            .Property(user => user.LUserCreate)
            .IsRequired()
            .ValueGeneratedNever();

        builder
            .Property(user => user.LUserEdit)
            .IsRequired()
            .ValueGeneratedNever();

        builder
            .Property(user => user.TCreate)
            .IsRequired()
            .ValueGeneratedNever();

        builder
            .Property(user => user.TEdit)
            .IsRequired()
            .ValueGeneratedNever();

        builder
            .Property(user => user.State)
            .IsRequired()
            .ValueGeneratedNever();

        builder
            .Property(user => user.FundName)
            .IsUnicode(true)
            .IsRequired()
            .ValueGeneratedNever();

        builder
        .Property(user => user.Password)
        .IsUnicode(true)
        .IsRequired()
        .ValueGeneratedNever();

        builder
            .Property(user => user.FundCode)
            .IsRequired()
            .ValueGeneratedNever();

        builder
       .Property(user => user.Certificate);

        builder
           .HasIndex(user => user.FundCode)
           .IsUnique();

    }
}