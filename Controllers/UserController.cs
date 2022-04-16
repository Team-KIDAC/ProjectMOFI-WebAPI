using Microsoft.AspNetCore.Mvc;
using API.MOFI_2.CustomExceptions;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using API.MOFI_2.Models;

namespace API.MOFI.Controllers {
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase {
        private readonly IConfiguration _config;
        IWebHostEnvironment _webHostEnvironment;
        MongoConnection _connection;

        private string blobConnectionString;

        public UserController(IWebHostEnvironment env, IConfiguration config) {
            _config = config;
            _webHostEnvironment = env;
            _connection = new MongoConnection(config);

            blobConnectionString = _config.GetValue<string>("BlobDetails:ConnectionString");
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers() {
            try {
                //System.Diagnostics.Process.Start(".");
                //Response.Redirect(".");

                //var py = Python.CreateEngine();
                //try {
                //    py.ExecuteFile("MOFIconsole.py");
                //}
                //catch (Exception ex) {
                //    Console.WriteLine("Oops");
                //    //Console.WriteLine(ex.StackTrace);
                //}

                return await Task.FromResult(Ok(_connection.LoadRecords()));
            }
            catch (Exception ex) {
                Console.WriteLine(ex.StackTrace);
                return StatusCode(500);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(string id) {
            try {
                if (string.IsNullOrWhiteSpace(id)) {
                    return BadRequest();
                }
                return await Task.FromResult(Ok(_connection.LoadRecordById(id)));
            }
            catch (InvalidIdException ex) {
                return StatusCode(404, ex.Message);
            }
            catch (Exception ex) {
                Console.WriteLine(ex.StackTrace);
                return StatusCode(500, "[ERROR]- An unexpected error occured!");
            }
        }

        [HttpPost]
        public async Task<IActionResult> PostUser(User newUser) {
            try {
                _connection.InsertUser(newUser);
                return await Task.FromResult(Created("", newUser));
            }
            catch (Exception ex) {
                Console.WriteLine(ex.StackTrace);
                return StatusCode(500, "[ERROR]- An unexpected error occured!");
            }
        }

        [HttpPost("{id}/Image")]
        public async Task<IActionResult> PostUserImage(IFormFileCollection newFiles, string id) {
            string uploads = Path.Combine(_webHostEnvironment.ContentRootPath, "uploads", id);
            Directory.CreateDirectory(uploads);

            for (int i = 0; i < newFiles.Count; i++) {
                string filePath = Path.Combine(uploads, id + "_" + i.ToString() + ".jpg");
                using (Stream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write)) {
                    newFiles[i].CopyTo(fileStream);
                }
            }

            return await Task.FromResult(Created("", newFiles));
        }
        
        [HttpPost("RecognizeImage")]
        public async Task<IActionResult> GetRecognizedImageId(IFormFile newFile) {
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(blobConnectionString);

            //string uploads = Path.Combine(_webHostEnvironment.ContentRootPath, "uploads", "RecognizableImage");
            //Directory.CreateDirectory(uploads);

            var cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();

            var cloudBlobContainer = cloudBlobClient.GetContainerReference("mofiimages");

            string newFileName = "RecognizableImage";
            var cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(newFileName);

            cloudBlockBlob.Properties.ContentType = newFile.ContentType;

            await cloudBlockBlob.UploadFromStreamAsync(newFile.OpenReadStream());

            //for (int i = 0; i < newFiles.Count; i++) {
            //    var cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();

            //    var cloudBlobContainer = cloudBlobClient.GetContainerReference("mofiimages");

            //    string newFileName = "RecognizableImage_" + i.ToString();
            //    var cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(newFileName);

            //    cloudBlockBlob.Properties.ContentType = newFiles[i].ContentType;

            //    await cloudBlockBlob.UploadFromStreamAsync(newFiles[i].OpenReadStream());

                //string filePath = Path.Combine(uploads, "RecognizableImage.jpg");
                //using (Stream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write)) {
                //    newFiles[i].CopyTo(fileStream);
                //}
            //}

            return await Task.FromResult(Created("", newFile.ContentType));
        }
        
        [HttpPost("RecognizeImageTest")]
        public async Task<IActionResult> PostRecognizableImageTest(ImageDuo imageDuo) {

            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(blobConnectionString);
            CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("mofiimages");

            string newFileName = "RecognizableImage.jpg";
            var cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(newFileName);
            cloudBlockBlob.Properties.ContentType = "image/jpeg";

            using (var stream = new MemoryStream(Convert.FromBase64String(imageDuo.Base64String))) {
                await cloudBlockBlob.UploadFromStreamAsync(stream);
            }

            //string uploads = Path.Combine(_webHostEnvironment.ContentRootPath, "uploads", "RecognizableImage");
            //string filePath = Path.Combine(uploads, "RecognizableImage.jpg");

            //System.IO.File.WriteAllBytes(filePath, Convert.FromBase64String(imageDuo.Base64String));
            //FileStream Stream = System.IO.File.OpenRead(filePath);

            //await cloudBlockBlob.UploadFromStreamAsync(Stream);

            return await Task.FromResult(Created("", "E1"));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id) {
            try {
                _connection.DeleteRecord(id);
                return await Task.FromResult(NoContent());
            }
            catch (ArgumentException ex) {
                Console.WriteLine(ex.StackTrace);
                return StatusCode(404, ex.Message);
            }
        }

        [HttpGet("{id}/Department")]
        public async Task<IActionResult> GetUserDepartment(string id) {
            try {
                if (string.IsNullOrWhiteSpace(id)) {
                    return BadRequest();
                }
                return await Task.FromResult(Ok(_connection.LoadRecordById(id).Department));
            }
            catch (InvalidIdException ex) {
                return StatusCode(404, ex.Message);
            }
            catch (Exception ex) {
                Console.WriteLine(ex.StackTrace);
                return StatusCode(500, "[ERROR]- An unexpected error occured!");
            }
        }
        [HttpGet("{id}/Vaccine")]
        public async Task<IActionResult> GetUserVaccine(string id) {
            try {
                return await Task.FromResult(Ok(_connection.LoadRecordById(id).Vaccine));
            }
            catch (InvalidIdException ex) {
                return StatusCode(404, ex.Message);
            }
            catch (Exception ex) {
                Console.WriteLine(ex.StackTrace);
                return StatusCode(500, "[ERROR]- An unexpected error occured!");
            }

            //if (!Array.Exists(users, user => user.Id == id )) return NotFound();
            //else return Ok(users[Array.FindIndex(users, user => user.Id == id)].Vaccine);

        }


    }
}
