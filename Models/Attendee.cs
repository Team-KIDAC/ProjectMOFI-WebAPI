using MongoDB.Bson.Serialization.Attributes;

namespace ProjectMOFI_Server_WebAPI.Models {

    public class Attendee {
        [BsonId]
        public string Id { get; init; }
        public string Name { get; init; }
        public string Department { get; init; }
        public string Vaccine { get; init; }
    }
}
