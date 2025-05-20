namespace CompanyEmployees.Core.Domain.Exceptions;

public class EmployeeNotFoundException : NotFoundException
{
    public EmployeeNotFoundException(Guid employeeId)
        : base($"Employee with id: {employeeId} does not exist in the database.")
    {
    }
}