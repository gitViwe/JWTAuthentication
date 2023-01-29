using Shared.Contract.Identity;

namespace Application.Feature.Identity.RefreshToken;

public class RefreshTokenCommand : TokenRequest, IRequest<TokenResponse> { }

internal class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, TokenResponse>
{
    private readonly IHubIdentityService _hubIdentity;

    public RefreshTokenCommandHandler(IHubIdentityService hubIdentity)
    {
        _hubIdentity = hubIdentity;
    }

    public async Task<TokenResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        return await _hubIdentity.RefreshToken(request, cancellationToken);
    }
}
