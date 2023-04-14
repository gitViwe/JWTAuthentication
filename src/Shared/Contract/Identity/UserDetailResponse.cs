using Shared.Contract.ApiClient;

namespace Shared.Contract.Identity;

public class UserDetailResponse
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public UserDetailProfileImage ProfileImage { get; set; } = new();
}

public class UserDetailProfileImage
{
    public ImgBBImage Image { get; set; } = new();
    public ImgBBThumb Thumbnail { get; set; } = new();
}