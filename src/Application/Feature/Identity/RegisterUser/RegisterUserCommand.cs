using Shared.Contract.Identity;

namespace Application.Feature.Identity.RegisterUser;

public class RegisterUserCommand : RegisterRequest, IRequest<IResponse> { }

internal class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, IResponse>
{
    private readonly IHubIdentityService _hubIdentity;

    public RegisterUserCommandHandler(IHubIdentityService hubIdentity)
    {
        _hubIdentity = hubIdentity;
    }

    public async Task<IResponse> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        return await _hubIdentity.RegisterAsync(request, cancellationToken);
    }
}
