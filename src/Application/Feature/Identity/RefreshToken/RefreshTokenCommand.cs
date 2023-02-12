using Shared.Contract.Identity;

namespace Application.Feature.Identity.RefreshToken;

public class RefreshTokenCommand : TokenRequest, IRequest<ITokenResponse> { }

internal class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, ITokenResponse>
{
    private readonly IHubIdentityService _hubIdentity;

    public RefreshTokenCommandHandler(IHubIdentityService hubIdentity)
    {
        _hubIdentity = hubIdentity;
    }

    public async Task<ITokenResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        return await _hubIdentity.RefreshToken(request, cancellationToken);
    }
}
