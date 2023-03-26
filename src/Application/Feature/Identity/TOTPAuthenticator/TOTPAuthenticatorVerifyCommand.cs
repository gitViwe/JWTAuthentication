using Shared.Contract.Identity;

namespace Application.Feature.Identity.TOTPAuthenticator;

public class TOTPAuthenticatorVerifyCommand : TOTPVerifyRequest, IRequest<IResponse>
{
    public string Email { get; set; } = string.Empty;
}

public class TOTPAuthenticatorVerifyCommandHandler : IRequestHandler<TOTPAuthenticatorVerifyCommand, IResponse>
{
    private readonly IHubIdentityService _hubIdentity;

    public TOTPAuthenticatorVerifyCommandHandler(IHubIdentityService hubIdentity)
    {
        _hubIdentity = hubIdentity;
    }
    public Task<IResponse> Handle(TOTPAuthenticatorVerifyCommand request, CancellationToken cancellationToken)
    {
        return _hubIdentity.ValidateTOTPAsync(request.Email, request.Token, cancellationToken);
    }
}
