using System.ComponentModel.DataAnnotations.Schema;

namespace WinDiet.Models;

public class Professional
{
    // Construtor para o EF (vazio)
    public Professional() { }

    public Professional(Guid userId, string crn, string specialty, string bio = "")
    {
        Id = Guid.NewGuid();
        UserId = userId;
        CRN = crn;
        Specialty = specialty;
        Bio = bio;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    [ForeignKey("UserId")]
    public virtual User User { get; set; }

    public string CRN { get; set; }
    public string Specialty { get; set; }
    public string Bio { get; set; }
    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Patient> Patients { get; set; } = new List<Patient>();
}