using Shared.Contract.Identity;
using System.Security.Claims;

namespace Application.Common.Interface;

public interface IJWTTokenService
{
    TokenResponse GenerateToken(ClaimsPrincipal claimsPrincipal);
}
