using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WinDiet.Models;

namespace WinDiet.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);
        
        builder.Property(u => u.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(150);

        builder.HasIndex(u => u.Email)
            .IsUnique();
        
        // Configuração do Relacionamento 1:1 com Professional
        builder.HasOne(u => u.Professional)
            .WithOne(p => p.User)
            .HasForeignKey<Professional>(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configuração do Relacionamento 1:1 com Patient
        builder.HasOne(u => u.Patient)
            .WithOne(p => p.User)
            .HasForeignKey<Patient>(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}