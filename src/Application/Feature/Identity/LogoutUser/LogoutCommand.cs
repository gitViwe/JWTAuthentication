namespace Application.Feature.Identity.LogoutUser;

public record LogoutCommand(string TokenId) : IRequest { }

public class LogoutCommandHandler : IRequestHandler<LogoutCommand>
{
    private readonly IHubIdentityService _hubIdentity;

    public LogoutCommandHandler(IHubIdentityService hubIdentity)
    {
        _hubIdentity = hubIdentity;
    }

    public async Task<Unit> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        await _hubIdentity.LogoutUserAsync(request.TokenId, cancellationToken);

        return Unit.Value;
    }
}
