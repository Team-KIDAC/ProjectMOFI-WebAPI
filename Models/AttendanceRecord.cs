using MongoDB.Bson.Serialization.Attributes;

namespace ProjectMOFI_Server_WebAPI.Models {
    public class AttendanceRecord {
        [BsonId]
        public string AttendanceRecordId { get; init; }
        public string AttendeeId{ get; init; }
        public string AttendeeName { get; init; }
        public string AttendanceDate { get; init; }
        public string AttendanceTime { get; init; }

    }
}
