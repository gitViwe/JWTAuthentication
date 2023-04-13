using Application.Service;
using Shared.Contract.Identity;

namespace Application.Feature.Identity.UpdateUser;

public class UpdateUserRequestCommand : UpdateUserRequest, IRequest<IResponse<TokenResponse>>
{
    public string Email { get; set; } = string.Empty;
}

public class UpdateUserRequestCommandHandler : IRequestHandler<UpdateUserRequestCommand, IResponse<TokenResponse>>
{
    private readonly IHubIdentityService _identityService;

    public UpdateUserRequestCommandHandler(IHubIdentityService identityService)
    {
        _identityService = identityService;
    }

    public Task<IResponse<TokenResponse>> Handle(UpdateUserRequestCommand request, CancellationToken cancellationToken)
    {
        return _identityService.UpdateUserAsync(request.Email, request, cancellationToken);
    }
}