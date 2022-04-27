using MongoDB.Bson;
using MongoDB.Driver;
using ProjectMOFI_Server_WebAPI.Models;

namespace ProjectMOFI_Server_WebAPI.MongoDB {
    public class MongoConnection {
        private readonly IConfiguration _config;

        private IMongoDatabase db;
        private string databaseName;
        private string userCollectionName;
        private string attendaceRecordCollectionName = "AttendanceRecords";

        public MongoConnection(IConfiguration config) {
            _config = config;

            databaseName = _config["MongoDetails-DatabaseName"];
            userCollectionName = _config["MongoDetails-CollectionName"];

            var client = new MongoClient(_config["MongoDetails-ConnectionString"]);
            db = client.GetDatabase(databaseName);
        }

        public void InsertUser(Attendee newAttendee) {
            var collection = db.GetCollection<Attendee>(userCollectionName);
            collection.InsertOne(newAttendee);
        }

        public List<Attendee> LoadUsers() {
            var collection = db.GetCollection<Attendee>(userCollectionName);

            return collection.Find(new BsonDocument()).ToList();
        }

        public Attendee LoadUserById(string id) {
            var collection = db.GetCollection<Attendee>(userCollectionName);
            var filter = Builders<Attendee>.Filter.Eq("Id", id);

            if (collection.Find(filter).Any()) {
                return collection.Find(filter).First();
            }
            else {
                throw new ArgumentException("[ERROR] - Invalid ID provided.");
            }
        }

        public void DeleteUser(string id) {
            var collection = db.GetCollection<Attendee>(userCollectionName);
            var filter = Builders<Attendee>.Filter.Eq("Id", id);
            if (collection.Find(filter).Any()) {
                collection.DeleteOne(filter);
            }
            else {
                throw new ArgumentException("[Error] - No such ID available.");
            }

        }

        public void InsertAttendaceRecord(AttendanceRecord newAttendanceRecord) {
            var collection = db.GetCollection<AttendanceRecord>(attendaceRecordCollectionName);
            collection.InsertOne(newAttendanceRecord);
        }

        public List<AttendanceRecord> LoadAttendaceRecords() {
            var collection = db.GetCollection<AttendanceRecord>(attendaceRecordCollectionName);

            return collection.Find(new BsonDocument()).ToList();
        }
    }
}
