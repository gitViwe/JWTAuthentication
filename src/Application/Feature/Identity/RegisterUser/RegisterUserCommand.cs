using Shared.Contract.Identity;

namespace Application.Feature.Identity.RegisterUser;

public class RegisterUserCommand : RegisterRequest, IRequest<TokenResponse> { }

internal class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, TokenResponse>
{
    private readonly IHubIdentityService _hubIdentity;

    public RegisterUserCommandHandler(IHubIdentityService hubIdentity)
    {
        _hubIdentity = hubIdentity;
    }

    public async Task<TokenResponse> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        return await _hubIdentity.RegisterAsync(request, cancellationToken);
    }
}
