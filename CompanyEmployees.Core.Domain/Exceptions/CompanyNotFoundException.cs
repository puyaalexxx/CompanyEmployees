namespace CompanyEmployees.Core.Domain.Exceptions;

public sealed class CompanyNotFoundException : NotFoundException
{
    public CompanyNotFoundException(Guid companyId) 
        : base($"The company with id: {companyId} deos not exist in the database.")
    {
    }
}