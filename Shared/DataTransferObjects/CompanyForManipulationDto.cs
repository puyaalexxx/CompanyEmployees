using System.ComponentModel.DataAnnotations;

namespace Shared.DataTransferObjects
{
    public record CompanyForManipulationDto
    {
        [Required(ErrorMessage = "Company Name is a required field.")]
        [MaxLength(60, ErrorMessage = "Maximum length for the Name is 60 characters.")]
        public string? Name { get; init; }

        [Required(ErrorMessage = "Company Address is a required field.")]
        [MaxLength(60, ErrorMessage = "Maximum length for the Address is 60 characters.")]
        public string? Address { get; init; }

        public string? Country { get; init; }

        public IEnumerable<EmployeeForCreationDto>? Employees { get; init; }
    }
}
