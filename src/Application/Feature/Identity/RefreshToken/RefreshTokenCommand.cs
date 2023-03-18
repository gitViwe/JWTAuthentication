using Shared.Contract.Identity;

namespace Application.Feature.Identity.RefreshToken;

public class RefreshTokenCommand : TokenRequest, IRequest<IResponse<TokenResponse>> { }

internal class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, IResponse<TokenResponse>>
{
    private readonly IHubIdentityService _hubIdentity;

    public RefreshTokenCommandHandler(IHubIdentityService hubIdentity)
    {
        _hubIdentity = hubIdentity;
    }

    public async Task<IResponse<TokenResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        return await _hubIdentity.RefreshToken(request, cancellationToken);
    }
}
