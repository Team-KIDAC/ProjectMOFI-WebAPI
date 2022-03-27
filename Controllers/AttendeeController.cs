using Microsoft.AspNetCore.Mvc;
using ProjectMOFI_Server_WebAPI.MongoDB;

namespace ProjectMOFI_Server_WebAPI.Controllers {
    [Route("[controller]")]
    [ApiController]
    public class AttendeeController : ControllerBase {

        MongoConnection _connection;

        public AttendeeController(IConfiguration config) {
            _connection = new MongoConnection(config);
        }

        [HttpGet]
        public async Task<IActionResult> GetAttendees() {
            try {
                return await Task.FromResult(Ok(_connection.LoadRecords()));
            }
            catch (Exception ex) {
                Console.WriteLine(ex.StackTrace);
                return StatusCode(500);
            }
        }
    }
}
