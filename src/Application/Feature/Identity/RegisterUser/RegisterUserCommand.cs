using Application.Service;
using Shared.Contract.Identity;

namespace Application.Feature.Identity.RegisterUser;

public class RegisterUserCommand : RegisterRequest, IRequest<IResponse<TokenResponse>> { }

internal class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, IResponse<TokenResponse>>
{
    private readonly IHubIdentityService _hubIdentity;

    public RegisterUserCommandHandler(IHubIdentityService hubIdentity)
    {
        _hubIdentity = hubIdentity;
    }

    public Task<IResponse<TokenResponse>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        return _hubIdentity.RegisterAsync(request, cancellationToken);
    }
}
