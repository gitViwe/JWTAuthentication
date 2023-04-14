using Infrastructure.Persistence.Configuration;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using Shared.Constant;

namespace Infrastructure.Persistence;

/// <summary>
/// A mongoDb database repository to interface with the document database
/// </summary>
/// <typeparam name="TMongoDocument">The document database entity type</typeparam>
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

    /// <summary>
    /// Gets the first result or null
    /// </summary>
    /// <param name="id">The <seealso cref="ObjectId.ToString"/> value</param>
    /// <returns>A Task whose result is the single result or null</returns>
    public Task<TMongoDocument> FindByIdAsync(string id)
    {
        var objectId = new ObjectId(id);
        var filter = Builders<TMongoDocument>.Filter.Eq(doc => doc.Id, objectId);
        return _collection.Find(filter).FirstOrDefaultAsync();
    }

    /// <summary>
    /// Replace a single document or create an new one if it does not exist
    /// </summary>
    /// <param name="document">The document to insert or replace</param>
    /// <returns>A Task representing the method</returns>
    public Task ReplaceOneAsync(TMongoDocument document)
    {
        var filter = Builders<TMongoDocument>.Filter.Eq(doc => doc.Id, document.Id);
        return _collection.ReplaceOneAsync(filter, document, new ReplaceOptions { IsUpsert = true });
    }

    /// <summary>
    /// Creates a queryable source of documents.
    /// </summary>
    /// <returns>Creates a queryable source of documents.</returns>
    public IQueryable<TMongoDocument> AsQueryable()
    {
        return _collection.AsQueryable();
    }
}
