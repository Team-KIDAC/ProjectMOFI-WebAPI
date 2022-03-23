using MongoDB.Bson;
using MongoDB.Driver;
using ProjectMOFI_Server_WebAPI.Models;

namespace ProjectMOFI_Server_WebAPI.MongoDB {
    public class MongoConnection {
        private readonly IConfiguration _config;

        private IMongoDatabase db;
        private string databaseName;
        private string collectionName;

        public MongoConnection(IConfiguration config) {
            _config = config;

            databaseName = _config.GetValue<string>("MongoDetails:DatabaseName");
            collectionName = _config.GetValue<string>("MongoDetails:CollectionName");

            var client = new MongoClient(_config.GetValue<string>("MongoDetails:ConnectionString"));
            db = client.GetDatabase(databaseName);
        }

        public void InsertUser(Attendee newAttendee) {
            var collection = db.GetCollection<Attendee>(collectionName);
            collection.InsertOne(newAttendee);
        }

        public List<Attendee> LoadRecords() {
            var collection = db.GetCollection<Attendee>(collectionName);

            return collection.Find(new BsonDocument()).ToList();
        }

        public Attendee LoadRecordById(string id) {
            var collection = db.GetCollection<Attendee>(collectionName);
            var filter = Builders<Attendee>.Filter.Eq("Id", id);

            if (collection.Find(filter).Any()) {
                return collection.Find(filter).First();
            }
            else {
                throw new ArgumentException("[ERROR] - Invalid ID provided.");
            }

        }

        public void DeleteRecord(string id) {
            var collection = db.GetCollection<Attendee>(collectionName);
            var filter = Builders<Attendee>.Filter.Eq("Id", id);
            if (collection.Find(filter).Any()) {
                collection.DeleteOne(filter);
            }
            else {
                throw new ArgumentException("[Error] - No such ID available.");
            }

        }
    }
}
