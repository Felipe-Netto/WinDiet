using WinDiet.Enums;

namespace WinDiet.DTOs;

public record RegisterPatientDTO(
    string Name,
    string Email,
    string Password,
    DateTime BirthDate,
    Gender Gender,
    decimal Height,
    decimal TargetWeight,
    string Observations
);