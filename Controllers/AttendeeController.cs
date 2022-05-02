using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using ProjectMOFI_Server_WebAPI.Models;
using ProjectMOFI_Server_WebAPI.MongoDB;

namespace ProjectMOFI_Server_WebAPI.Controllers {
    [Route("[controller]")]
    [ApiController, Authorize(Roles = "Student")]
    public class AttendeeController : ControllerBase {

        IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _config;
        MongoConnection _connection;

        private string blobConnectionString;

        public AttendeeController(IWebHostEnvironment env, IConfiguration config) {
            _config = config;
            _webHostEnvironment = env;
            _connection = new MongoConnection(config);

            blobConnectionString = _config["BlobDetails-ConnectionString"];
        }

        [HttpGet]
        public async Task<IActionResult> GetAttendees() {
            try {
                return await Task.FromResult(Ok(_connection.LoadUsers()));
            }
            catch (Exception ex) {
                Console.WriteLine(ex.StackTrace);
                return StatusCode(500);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAttendeeById(string id) {
            try {
                if (string.IsNullOrWhiteSpace(id)) {
                    return BadRequest();
                }
                return await Task.FromResult(Ok(_connection.LoadUserById(id)));
            }
            catch (ArgumentException ex) {
                return StatusCode(404, ex.Message);
            }
            catch (Exception ex) {
                Console.WriteLine(ex.StackTrace);
                return StatusCode(500, "[ERROR]- An unexpected error occured!");
            }
        }
        
        [HttpGet("{id}/ImagePath")]
        public async Task<IActionResult> GetImagePathOfAttendee(string id) {
            try {
                if (string.IsNullOrWhiteSpace(id)) {
                    return BadRequest();
                }
                return await Task.FromResult(Ok(_connection.LoadUserImagePathById(id)));
            }
            catch (ArgumentException ex) {
                return StatusCode(404, ex.Message);
            }
            catch (Exception ex) {
                Console.WriteLine(ex.StackTrace);
                return StatusCode(500, "[ERROR]- An unexpected error occured!");
            }
        }

        [HttpPost("UploadAttendeeImage")]
        public async Task<IActionResult> PostAttendeeImage(AttendeeImage attendeeImage) {
            try {
                CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(blobConnectionString);
                CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
                CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("mofi-user-images");

                string newFileName = attendeeImage.AttendeeId + "_UserImage.jpg";
                var cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(newFileName);
                cloudBlockBlob.Properties.ContentType = "image/jpeg";

                using (var stream = new MemoryStream(Convert.FromBase64String(attendeeImage.Base64String))) {
                    await cloudBlockBlob.UploadFromStreamAsync(stream);
                }

                string imageUploadedPath = "https://mofiblob.blob.core.windows.net/mofi-user-images/" + attendeeImage.AttendeeId + "_UserImage.jpg";
                return await Task.FromResult(Created("", imageUploadedPath));
            }
            catch (Exception ex) {
                Console.WriteLine(ex.StackTrace);
                return StatusCode(500, "[ERROR]- An unexpected error occured!");
            }
        }

        [HttpPost]
        public async Task<IActionResult> PostAttendee(Attendee newAttendee) {
            try {
                _connection.InsertUser(newAttendee);
                return await Task.FromResult(Created("", newAttendee));
            }
            catch (Exception ex) {
                Console.WriteLine(ex.StackTrace);
                return StatusCode(500, "[ERROR]- An unexpected error occured!");
            }
        }

    }
}
