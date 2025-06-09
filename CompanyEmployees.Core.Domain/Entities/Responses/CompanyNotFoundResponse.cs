namespace CompanyEmployees.Core.Domain.Entities.Responses
{
    public sealed class CompanyNotFoundResponse : ApiNotFoundResponse
    {
        public CompanyNotFoundResponse(Guid companyId) : base($"Company with ID '{companyId}' not found in DB.")
        {
        }
    }
}
