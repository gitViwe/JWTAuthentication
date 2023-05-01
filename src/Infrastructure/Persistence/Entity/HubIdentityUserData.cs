using gitViwe.Shared.MongoDB.Base;
using Shared.Contract.ApiClient;

namespace Infrastructure.Persistence.Entity;

[BsonCollection(nameof(HubIdentityUserData))]
internal class HubIdentityUserData : MongoDocument
{
    public ImgBBUploadResponse ProfileImage { get; set; } = new();
}
