using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectMOFI_Server_WebAPI.Models;
using ProjectMOFI_Server_WebAPI.MongoDB;

namespace ProjectMOFI_Server_WebAPI.Controllers {
    [Route("[controller]")]
    [ApiController, Authorize(Roles = "Student")]
    public class AttendanceRecordController : ControllerBase {
        //The API Controller to handle the database operations about the attendance records.

        IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _config;
        MongoConnection _connection;

        public AttendanceRecordController(IWebHostEnvironment env, IConfiguration config) {
            _config = config;
            _webHostEnvironment = env;
            _connection = new MongoConnection(config);
        }

        [HttpGet]
        public async Task<IActionResult> GetAttendanceRecords() {
            //The HTTP method to return the details of all attendance records.

            try {
                return await Task.FromResult(Ok(_connection.LoadAttendaceRecords()));
            }
            catch (Exception ex) {
                Console.WriteLine(ex.StackTrace);
                return StatusCode(500);
            }
        }

        [HttpPost]
        public async Task<IActionResult> PostAttendanceRecord(AttendanceRecord newAttendanceRecord) {
            //The HTTP method to submit a new record to the attendance records.

            try {
                _connection.InsertAttendaceRecord(newAttendanceRecord);
                return await Task.FromResult(Created("", newAttendanceRecord));
            }
            catch (Exception ex) {
                Console.WriteLine(ex.StackTrace);
                return StatusCode(500, "[ERROR]- An unexpected error occured!");
            }
        }
    }
}
