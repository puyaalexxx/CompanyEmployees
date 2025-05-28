using FluentValidation;
using Shared.DataTransferObjects;

namespace CompanyEmployees.Infrastructure.Presentation.Validators
{
    public class EmployeeForManipulationDtoValidator<T> : AbstractValidator<T> where T : EmployeeForManipulationDto
    {
        public EmployeeForManipulationDtoValidator()
        {
            RuleFor(e => e.Name)
                .NotEmpty().NotNull().WithMessage("Employee name is a required field.")
                .MaximumLength(30).WithMessage("Maximum length for the Name is 30 characters.");

            RuleFor(e => e.Age)
                .NotEmpty().WithMessage("Employee age is a required field.")
                .GreaterThanOrEqualTo(18).WithMessage("Employee age must be at least 18 years old.");

            RuleFor(e => e.Position)
                .NotEmpty().NotNull().WithMessage("Employee position is a required field.")
                .MaximumLength(20).WithMessage("Maximum length for the Position is 20 characters.");
        }
    }
}
