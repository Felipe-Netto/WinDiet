using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WinDiet.Enums;

namespace WinDiet.Models;

public class Patient
{
    public Patient()
    {
    }

    public Patient(Guid userId, Guid professionalId, DateTime birthDate, Gender gender, decimal height, decimal targetWeight, String observations = "")
    {
        Id = Guid.NewGuid();
        UserId = userId;
        ProfessionalId = professionalId;
        BirthDate = birthDate;
        Gender = gender;
        Observations = observations;
        Height = height;
        TargetWeight = targetWeight;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    [Key] public Guid Id { get; set; }

    // Vinculação com o login do paciente
    [Required] public Guid UserId { get; set; }

    [ForeignKey("UserId")] public virtual User User { get; set; }

    // Vinculação com o profissional que o atende
    [Required] public Guid ProfessionalId { get; set; }

    [ForeignKey("ProfessionalId")]
    public virtual Professional Professional { get; set; }

    // Dados Biométricos para o MVP
    [Required] public DateTime BirthDate { get; set; }

    [Required] public Gender Gender { get; set; } // Enum (Male, Female)

    [Column(TypeName = "decimal(5,2)")] public decimal Height { get; set; } // Em cm ou metros (ex: 180.00 ou 1.80)

    [Column(TypeName = "decimal(5,2)")] public decimal TargetWeight { get; set; } // Peso meta para o ranking

    public string Observations { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}