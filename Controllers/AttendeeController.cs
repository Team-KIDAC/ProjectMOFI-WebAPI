using Microsoft.AspNetCore.Mvc;

namespace ProjectMOFI_Server_WebAPI.Controllers {
    [Route("[controller]")]
    [ApiController]
    public class AttendeeController : ControllerBase {

        [HttpGet]
        public async Task<IActionResult> GetUsers() {
            try {
                return await Task.FromResult(Ok("Hooray!"));
            }
            catch (Exception ex) {
                Console.WriteLine(ex.StackTrace);
                return StatusCode(500);
            }
        }
    }
}
