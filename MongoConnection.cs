using MongoDB.Bson;
using MongoDB.Driver;
using Microsoft.Extensions.Configuration;
using API.MOFI_2.CustomExceptions;

namespace API.MOFI
{
    public class MongoConnection
    {
        private readonly IConfiguration _config;

        private IMongoDatabase db;
        private string databaseName;
        private string collectionName;

        public MongoConnection(IConfiguration config)
        {
            _config = config;

            databaseName = _config.GetValue<string>("MongoDetails:DatabaseName");
            collectionName = _config.GetValue<string>("MongoDetails:CollectionName");

            var client = new MongoClient(_config.GetValue<string>("MongoDetails:ConnectionString"));
            db = client.GetDatabase(databaseName);
        }

        public void InsertUser(User newUser)
        {
            var collection = db.GetCollection<User>(collectionName);
            collection.InsertOne(newUser);
        }

        public List<User> LoadRecords()
        {
            var collection = db.GetCollection<User>(collectionName);

            return collection.Find(new BsonDocument()).ToList();
        }

        public User LoadRecordById(string id)
        {
            var collection = db.GetCollection<User>(collectionName);
            var filter = Builders<User>.Filter.Eq("Id", id);

            if (collection.Find(filter).Any()) {
                return collection.Find(filter).First();
            } else {
                throw new InvalidIdException();
            }

        }

        public void DeleteRecord(string id)
        {
            var collection = db.GetCollection<User>(collectionName);
            var filter = Builders<User>.Filter.Eq("Id", id);
            if (collection.Find(filter).Any()) {
                collection.DeleteOne(filter);
            } else {
                throw new ArgumentException("[Error] - No such ID available.");
            }
            
        }
    }
}
