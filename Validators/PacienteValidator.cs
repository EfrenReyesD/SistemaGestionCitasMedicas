using FluentValidation;
using SistemaGestionCitasMedicas.Models;

namespace SistemaGestionCitasMedicas.Validators
{
    public class PacienteValidator : AbstractValidator<Paciente>
    {
        public PacienteValidator()
        {
            RuleFor(p => p.Nombre)
                .NotEmpty().WithMessage("El nombre es obligatorio")
                .MinimumLength(3).WithMessage("El nombre debe tener al menos 3 caracteres");

            RuleFor(p => p.Email)
                .NotEmpty().WithMessage("El email es obligatorio")
                .EmailAddress().WithMessage("El email no tiene un formato válido");

            RuleFor(p => p.FechaNacimiento)
                .LessThan(DateTime.Now).WithMessage("La fecha de nacimiento debe ser en el pasado");

            RuleFor(p => p.Telefono)
                .NotEmpty().WithMessage("El teléfono es obligatorio")
                .Matches(@"^\+?[0-9\s\-()]+$").WithMessage("El teléfono no tiene un formato válido");
        }
    }
}
