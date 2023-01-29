using Shared.Contract.Identity;

namespace Application.Feature.Identity.LoginUser;

public class LoginUserCommand : LoginRequest, IRequest<TokenResponse> { }

internal class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, TokenResponse>
{
    private readonly IHubIdentityService _hubIdentity;

    public LoginUserCommandHandler(IHubIdentityService hubIdentity)
    {
        _hubIdentity = hubIdentity;
    }

    public async Task<TokenResponse> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        return await _hubIdentity.LoginUserAsync(request, cancellationToken);
    }
}