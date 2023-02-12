using Shared.Contract.Identity;

namespace Application.Feature.Identity.RegisterUser;

public class RegisterUserCommand : RegisterRequest, IRequest<ITokenResponse> { }

internal class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, ITokenResponse>
{
    private readonly IHubIdentityService _hubIdentity;

    public RegisterUserCommandHandler(IHubIdentityService hubIdentity)
    {
        _hubIdentity = hubIdentity;
    }

    public async Task<ITokenResponse> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        return await _hubIdentity.RegisterAsync(request, cancellationToken);
    }
}
