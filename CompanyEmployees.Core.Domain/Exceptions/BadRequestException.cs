namespace CompanyEmployees.Core.Domain.Exceptions;

public abstract class BadRequestException : Exception
{
    protected BadRequestException(string message) : base(message)
    {
        
    }
}