using ProjectMOFI_Server_WebAPI.Models;
using System.Collections;

namespace ProjectMOFI_Server_WebAPI.Comparers {
    public class SortAttendanceRecordsByDateComparer : IComparer<AttendanceRecord> {
        public int Compare(AttendanceRecord x, AttendanceRecord y) {
            return x.AttendanceDate.CompareTo(y.AttendanceDate);
            //throw new NotImplementedException();
        }
    }
}
