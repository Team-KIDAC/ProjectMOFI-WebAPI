using Microsoft.AspNetCore.Mvc;
using ProjectMOFI_Server_WebAPI.MongoDB;

namespace ProjectMOFI_Server_WebAPI.Controllers {
    [Route("[controller]")]
    [ApiController]
    public class AttendeeController : ControllerBase {

        IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _config;
        MongoConnection _connection;

        public AttendeeController(IWebHostEnvironment env, IConfiguration config) {
            _config = config;
            _webHostEnvironment = env;
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
