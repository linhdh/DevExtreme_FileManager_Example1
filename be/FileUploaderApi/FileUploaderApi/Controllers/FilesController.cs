using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IO;

namespace FileUploaderApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public FilesController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public IActionResult UploadChunk([FromForm] IFormFile file, [FromForm] string chunkMetadata)
        {
            var tempPath = _configuration["TempFolder"];

            RemoveTempFilesAfterDelay(tempPath!, new TimeSpan(0, 5, 0));

            var metaDataObject = JsonConvert.DeserializeObject<ChuckMetadata>(chunkMetadata);

            var fileName = metaDataObject.FileName.ToLower();

            string[] imageExtensions = { ".zip" };

            var isValidExtenstion = imageExtensions.Any(ext =>
            {
                return fileName.LastIndexOf(ext) > -1;
            });

            if (!isValidExtenstion)
            {
                throw new Exception("Not allowed file extension.");
            }

            var tempFilePath = Path.Combine(tempPath!, metaDataObject.FileGuid + ".tmp");

            if (!Directory.Exists(tempPath))
            {
                Directory.CreateDirectory(tempPath!);
            }

            using (var stream = new FileStream(tempFilePath, FileMode.Append, FileAccess.Write))
            {
                file.CopyTo(stream);
                if (stream.Length > 1024 * 1024 * 1024)
                {
                    throw new Exception("File is too large");
                }
            }

            if (metaDataObject.Index == metaDataObject.TotalCount - 1)
            {
                System.IO.File.Copy(tempFilePath, Path.Combine(_configuration["TempFolder"]!, metaDataObject.FileName));
            }

            return Ok();
        }

        private void RemoveTempFilesAfterDelay(string path, TimeSpan delay)
        {
            var dir = new DirectoryInfo(path);
            if (dir.Exists)
            {
                foreach (var file in dir.GetFiles("*.tmp").Where(f => f.LastWriteTimeUtc.Add(delay) < DateTime.UtcNow))
                {
                    file.Delete();
                }
            }
        }        
    }
}
