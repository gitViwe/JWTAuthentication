using Infrastructure.Persistence.Configuration;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using Shared.Constant;

namespace Infrastructure.Persistence;

internal class MongoDBRepository<TMongoDocument> where TMongoDocument : MongoDocument
{
    private readonly IMongoCollection<TMongoDocument> _collection;

    public MongoDBRepository(IConfiguration configuration)
    {
        var database = new MongoClient(configuration.GetConnectionString(HubConfigurations.ConnectionString.MongoDb)!).GetDatabase("hub-db");
        _collection = database.GetCollection<TMongoDocument>(GetCollectionName(typeof(TMongoDocument)));
    }

    private protected string GetCollectionName(Type documentType)
    {
        var data = documentType.GetCustomAttributes(typeof(BsonCollectionAttribute), true).FirstOrDefault()
            ?? throw new ArgumentException($"'{nameof(documentType)}' does not have the class attribute/decorator '{nameof(BsonCollectionAttribute)}'");

        return ((BsonCollectionAttribute)data).CollectionName;
    }

    public Task<TMongoDocument> FindByIdAsync(string id)
    {
        var objectId = new ObjectId(id);
        var filter = Builders<TMongoDocument>.Filter.Eq(doc => doc.Id, objectId);
        return _collection.Find(filter).FirstOrDefaultAsync();
    }

    public Task ReplaceOneAsync(TMongoDocument document)
    {
        var filter = Builders<TMongoDocument>.Filter.Eq(doc => doc.Id, document.Id);
        return _collection.ReplaceOneAsync(filter, document, new ReplaceOptions { IsUpsert = true });
    }
}
