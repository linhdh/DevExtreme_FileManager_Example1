using DevExtreme.AspNet.Mvc.FileManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Semantics;

namespace FileManagerAPI.Controllers
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

        [AcceptVerbs("POST")]
        [Route("manage")]
        [Consumes("multipart/form-data")]
        public IActionResult FileSystemPost([FromForm] FileSystemCommand command, [FromForm] string arguments)
        {
            var config = new FileSystemConfiguration()
            {
                Request = HttpContext.Request,
                FileSystemProvider = new PhysicalFileSystemProvider(_configuration["FileStore"]),
                AllowDelete = true,
                AllowUpload = true,
                //AllowDownload = true,
                AllowedFileExtensions = new[] { ".zip", ".klib" },
                TempDirectory = _configuration["TempFolder"],
                UploadConfiguration = new UploadConfiguration() { ChunkSize = 102400, MaxFileSize = 1024 * 1024 * 1024 },
            };
            var processor = new FileSystemCommandProcessor(config);
            var commandResult = processor.Execute(command, arguments);
            if (commandResult.Success)
            {
                var result = commandResult.GetClientCommandResult();
                return Ok(result);
            }
            else
            {
                var result = commandResult.GetClientCommandResult();
                return BadRequest(result);
            }
        }

        [HttpGet("manage")]
        public IActionResult FileSystemGet(FileSystemCommand command, string arguments)
        {
            var config = new FileSystemConfiguration()
            {
                Request = HttpContext.Request,
                FileSystemProvider = new PhysicalFileSystemProvider(_configuration["FileStore"]),
                AllowDelete = true,
                AllowUpload = true,
                //AllowDownload = true,
                AllowedFileExtensions = new[] { ".zip", ".klib" },
                TempDirectory = _configuration["TempFolder"],
                UploadConfiguration = new UploadConfiguration() { ChunkSize = 102400, MaxFileSize = 1024 * 1024 * 1024 },
            };
            var processor = new FileSystemCommandProcessor(config);
            var commandResult = processor.Execute(command, arguments);
            if (commandResult.Success)
            {
                var result = commandResult.GetClientCommandResult();
                return Ok(result);
            }
            else
            {
                var result = commandResult.GetClientCommandResult();
                return BadRequest(result);
            }
        }
    }
}
