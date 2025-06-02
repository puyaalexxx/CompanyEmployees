namespace Shared.DataTransferObjects;

public record class EmployeeDto(Guid Id, string Name, int Age, string Position);