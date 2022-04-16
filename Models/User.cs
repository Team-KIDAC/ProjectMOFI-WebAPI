using MongoDB.Bson.Serialization.Attributes;

namespace API.MOFI
{
    public class User
    {
        [BsonId]
        public string Id { get; init; }
        public string Name { get; init; }
        public string Department { get; init; }
        public string Vaccine { get; init; } 

    }
}
