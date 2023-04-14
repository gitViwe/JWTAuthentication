using System.ComponentModel;

namespace Shared.Constant;

/// <summary>
/// Defines the permission values used by the 'Permission' claims policy
/// </summary>
public static class HubPermissions
{
    [DisplayName("Profile")]
    [Description("Profile Permission")]
    public static class Profile
    {
        public const string Manage = "Permission.Profile.Manage";
    }

    public static class Authentication
    {
        public const string VerifiedTOTP = "Permission.Authentication.VerifiedTOTP";
    }
}
