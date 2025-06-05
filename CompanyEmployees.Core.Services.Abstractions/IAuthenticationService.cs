using Microsoft.AspNetCore.Identity;
using Shared.DataTransferObjects;

namespace CompanyEmployees.Core.Services.Abstractions
{
    public interface IAuthenticationService
    {
        Task<IdentityResult> RegisterUser(UserForRegistrationDto userRegistration);

        Task<bool> ValidateUser(UserForAuthenticationDto userForAuthentication);

        Task<TokenDto> CreateToken(bool populateExp);

        Task<TokenDto> RefreshToken(TokenDto tokenDto);
    }
}
