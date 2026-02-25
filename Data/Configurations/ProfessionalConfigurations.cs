using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WinDiet.Models;

namespace WinDiet.Data.Configurations;

public class ProfessionalConfigurations : IEntityTypeConfiguration<Professional>
{
    public void Configure(EntityTypeBuilder<Professional> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.CRN)
            .IsRequired()
            .HasMaxLength(20);
        
        builder.Property(p => p.Specialty)
            .HasMaxLength(100);
        
        builder.Property(p => p.Bio)
            .HasMaxLength(500);

        builder.HasOne(p => p.User)
            .WithOne(u => u.Professional)
            .HasForeignKey<Professional>(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relacionamento 1:N com Patients (Um nutri tem muitos pacientes)
        builder.HasMany(p => p.Patients)
            .WithOne(pat => pat.Professional)
            .HasForeignKey(pat => pat.ProfessionalId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}