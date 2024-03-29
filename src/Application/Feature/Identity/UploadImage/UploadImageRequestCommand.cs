﻿using Application.Service;
using Shared.Contract.Identity;

namespace Application.Feature.Identity.UploadImage;

public class UploadImageRequestCommand : UploadImageRequest, IRequest<IResponse>
{
    public string UserId { get; set; } = string.Empty;
}

public class UploadImageRequestCommandHandler : IRequestHandler<UploadImageRequestCommand, IResponse>
{
    private readonly IHubIdentityService _identityService;

    public UploadImageRequestCommandHandler(IHubIdentityService identityService)
    {
        _identityService = identityService;
    }

    public Task<IResponse> Handle(UploadImageRequestCommand request, CancellationToken cancellationToken)
    {
        return _identityService.UploadImageAsync(request.UserId, request, cancellationToken);
    }
}