using Application.Common.Interface;
using Shared.Contract.Identity;

namespace Application.Feature.Identity.RefreshToken;

public class RefreshTokenCommand : TokenRequest, IRequest<IResponse> { }

internal class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, IResponse>
{
    private readonly IHubIdentityService _hubIdentity;

    public RefreshTokenCommandHandler(IHubIdentityService hubIdentity)
    {
        _hubIdentity = hubIdentity;
    }

    public async Task<IResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        return await _hubIdentity.RefreshToken(request, cancellationToken);
    }
}
