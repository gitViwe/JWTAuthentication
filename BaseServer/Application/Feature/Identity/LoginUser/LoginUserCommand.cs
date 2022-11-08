using Shared.Contract.Identity;

namespace Application.Feature.Identity.LoginUser;

public class LoginUserCommand : LoginRequest, IRequest<IResponse> { }

internal class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, IResponse>
{
    private readonly IHubIdentityService _hubIdentity;

    public LoginUserCommandHandler(IHubIdentityService hubIdentity)
    {
        _hubIdentity = hubIdentity;
    }

    public async Task<IResponse> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        return await _hubIdentity.LoginUserAsync(request, cancellationToken);
    }
}