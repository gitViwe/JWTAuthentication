using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Identity;

internal class RefreshTokenValidationParameters : TokenValidationParameters
{
	public RefreshTokenValidationParameters(TokenValidationParameters validationParameters)
		: base(validationParameters) { }
}
