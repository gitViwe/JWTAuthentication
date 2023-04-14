using Application.Service;
using Shared.Contract.Identity;

namespace Application.Feature.Identity.UserDetail;

public class UserDetailQuery : IRequest<IResponse<UserDetailResponse>>
{
    public string UserId { get; set; } = string.Empty;
}

public class UserDetailQueryHandler : IRequestHandler<UserDetailQuery, IResponse<UserDetailResponse>>
{
    private readonly IHubIdentityService _hubIdentity;

    public UserDetailQueryHandler(IHubIdentityService hubIdentity)
    {
        _hubIdentity = hubIdentity;
    }

    public Task<IResponse<UserDetailResponse>> Handle(UserDetailQuery request, CancellationToken cancellationToken)
    {
        return _hubIdentity.GetUserDetailAsync(request.UserId, cancellationToken);
    }
}
