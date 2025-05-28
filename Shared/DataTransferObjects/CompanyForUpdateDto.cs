namespace Shared.DataTransferObjects
{
    public record CompanyForUpdateDto(string Name, string Adress, string Country, IEnumerable<EmployeeForUpdateDto> Employees);
}
