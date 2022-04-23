using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Newtonsoft.Json.Linq;
using ProjectMOFI_Server_WebAPI.Models;
using ProjectMOFI_Server_WebAPI.MongoDB;

namespace ProjectMOFI_Server_WebAPI.Controllers {
    [Route("[controller]")]
    [ApiController]
    public class RecognitionController : ControllerBase {

        IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _config;
        MongoConnection _connection;

        private string blobConnectionString;

        public RecognitionController(IWebHostEnvironment env, IConfiguration config) {
            _config = config;
            _webHostEnvironment = env;
            _connection = new MongoConnection(config);

            blobConnectionString = _config["BlobDetails-ConnectionString"];
        }
        [HttpPost("RecognizePerson")]
        public async Task<IActionResult> PostRecognizePerson(ImageDuo imageDuo) {

            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(blobConnectionString);
            CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("mofiimages");

            string newFileName = "RecognizableImage.jpg";
            var cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(newFileName);
            cloudBlockBlob.Properties.ContentType = "image/jpeg";

            using (var stream = new MemoryStream(Convert.FromBase64String(imageDuo.Base64String))) {
                await cloudBlockBlob.UploadFromStreamAsync(stream);
            }

            JObject jasonObj;
            try {
                using (HttpClient client = new HttpClient()) {
                    Uri endpoint = new Uri("https://mofitestmlapi9.azurewebsites.net/");
                    HttpResponseMessage result = await client.GetAsync(endpoint);
                    if (result.StatusCode == System.Net.HttpStatusCode.InternalServerError) {
                        return StatusCode(500, "[ERROR]- An unexpected error occured in the Recognition API!");
                    }
                    if (result.StatusCode == System.Net.HttpStatusCode.NotFound) {
                        return StatusCode(404, "[NOTFOUND]- The person is not recognizable.");
                    }
                    string jsonStr = result.Content.ReadAsStringAsync().Result;

                    jasonObj = JObject.Parse(jsonStr);
                    //Console.WriteLine(jasonObj.GetValue("id"));
                }
                if (string.IsNullOrWhiteSpace(jasonObj.GetValue("id").ToString())) {
                    return BadRequest();
                }
                return await Task.FromResult(Ok(_connection.LoadRecordById(jasonObj.GetValue("id").ToString())));
            }
            catch (ArgumentException ex) {
                return StatusCode(404, ex.Message);
            }
            catch (Exception ex) {
                Console.WriteLine(ex.StackTrace);
                return StatusCode(500, "[ERROR]- An unexpected error occured!");
            }
            //return await Task.FromResult(Created("", "E1"));
        }
    }
}
