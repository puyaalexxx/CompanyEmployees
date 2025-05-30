namespace CompanyEmployees.Core.Domain.Exceptions
{
    public sealed class MaxAgeRangeBadRequestException : BadRequestException
    {
        public MaxAgeRangeBadRequestException() : base("Maximum age cannot be less than minimum age.")
        {
        }
    }
}
