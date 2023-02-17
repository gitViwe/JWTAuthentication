namespace Application.Feature.Identity.LogoutUser;

public record LogoutCommand(string TokenId) : IRequest { }

public class LogoutCommandHandler : IRequestHandler<LogoutCommand>
{
    private readonly IHubIdentityService _hubIdentity;

    public LogoutCommandHandler(IHubIdentityService hubIdentity)
    {
        _hubIdentity = hubIdentity;
    }

    public Task Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        return _hubIdentity.LogoutUserAsync(request.TokenId, cancellationToken);
    }
}
