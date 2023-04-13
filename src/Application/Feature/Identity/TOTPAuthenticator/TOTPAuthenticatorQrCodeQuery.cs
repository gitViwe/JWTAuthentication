using Application.Service;
using Shared.Contract.Identity;

namespace Application.Feature.Identity.TOTPAuthenticator;

public class TOTPAuthenticatorQrCodeQuery : QrCodeImageRequest, IRequest<IResponse<QrCodeImageResponse>> { }

public class TOTPAuthenticatorLinkCommandHandler : IRequestHandler<TOTPAuthenticatorQrCodeQuery, IResponse<QrCodeImageResponse>>
{
    private readonly IHubIdentityService _hubIdentity;

    public TOTPAuthenticatorLinkCommandHandler(IHubIdentityService hubIdentity)
    {
        _hubIdentity = hubIdentity;
    }

    public Task<IResponse<QrCodeImageResponse>> Handle(TOTPAuthenticatorQrCodeQuery request, CancellationToken cancellationToken)
    {
        return _hubIdentity.GetQrCodeImageAsync(request, cancellationToken);
    }
}
