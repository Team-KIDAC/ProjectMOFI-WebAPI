using MongoDB.Bson.Serialization.Attributes;

namespace ProjectMOFI_Server_WebAPI.Models {
    public class LoginUser {
        [BsonId]
        public string Username { get; set; } = string.Empty;
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
    }
}
