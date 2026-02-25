using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WinDiet.Models;

namespace WinDiet.Data.Configurations;

public class PatientConfigurations : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Height).HasPrecision(5, 2);
        builder.Property(p => p.TargetWeight).HasPrecision(5, 2);
        
        builder.Property(p => p.Observations)
            .HasMaxLength(1000);
        
        builder.Property(p => p.BirthDate)
            .IsRequired();
        
        builder.Property(p => p.IsActive)
            .HasDefaultValue(true);
        
        builder.Property(p => p.Gender)
            .IsRequired();

        builder.HasOne(p => p.User)
            .WithOne(u => u.Patient)
            .HasForeignKey<Patient>(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.Professional)
            .WithMany(p => p.Patients)
            .HasForeignKey(p => p.ProfessionalId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}