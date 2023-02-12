using Shared.Contract.Identity;

namespace Application.Feature.Identity.LoginUser;

public class LoginUserCommand : LoginRequest, IRequest<ITokenResponse> { }

internal class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, ITokenResponse>
{
    private readonly IHubIdentityService _hubIdentity;

    public LoginUserCommandHandler(IHubIdentityService hubIdentity)
    {
        _hubIdentity = hubIdentity;
    }

    public async Task<ITokenResponse> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        return await _hubIdentity.LoginUserAsync(request, cancellationToken);
    }
}