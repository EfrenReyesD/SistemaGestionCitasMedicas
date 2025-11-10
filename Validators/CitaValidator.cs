using FluentValidation;
using SistemaGestionCitasMedicas.Models;

namespace SistemaGestionCitasMedicas.Validators
{
    public class CitaValidator : AbstractValidator<Cita>
    {
        public CitaValidator()
        {
            RuleFor(c => c.FechaHora)
                .GreaterThan(DateTime.Now).WithMessage("La fecha de la cita debe ser en el futuro");

            RuleFor(c => c.DuracionMin)
                .GreaterThan(0).WithMessage("La duración debe ser mayor a 0 minutos")
                .LessThanOrEqualTo(480).WithMessage("La duración no puede exceder 8 horas");

            RuleFor(c => c.Tipo)
                .NotEmpty().WithMessage("El tipo de cita es obligatorio");

            RuleFor(c => c.Paciente)
                .NotNull().WithMessage("El paciente es obligatorio");

            RuleFor(c => c.Doctor)
                .NotNull().WithMessage("El doctor es obligatorio");
        }
    }
}
