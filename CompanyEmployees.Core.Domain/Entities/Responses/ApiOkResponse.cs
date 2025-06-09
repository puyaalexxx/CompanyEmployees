namespace CompanyEmployees.Core.Domain.Entities.Responses
{
    public sealed class ApiOkResponse<TResult> : ApiBaseResponse
    {
        public TResult Result { get; }

        public ApiOkResponse(TResult result) : base(true)
        {
            Result = result;
        }
    }
}
