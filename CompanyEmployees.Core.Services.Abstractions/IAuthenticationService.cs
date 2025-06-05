using Microsoft.AspNetCore.Identity;
using Shared.DataTransferObjects;

namespace CompanyEmployees.Core.Services.Abstractions
{
    public interface IAuthenticationService
    {
        Task<IdentityResult> RegisterUser(UserForRegistrationDto userRegistration);

        Task<bool> ValidateUser(UserForAuthenticationDto userForAuthentication);

        Task<string> CreateToken();
    }
}
