using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    public class UploadResult
    {
        public string FileName { get; set; }
        public int ErrorCode { get; set; }

        public string StoredFileName { get; set; }

        public bool Uploaded { get; set; }
    }

    [ApiController]
    [Route( "[controller]" )]
    public class FileSaveController : ControllerBase
    {
        private readonly IWebHostEnvironment env;
        private readonly ILogger<FileSaveController> logger;

        public FileSaveController( IWebHostEnvironment env,
            ILogger<FileSaveController> logger )
        {
            this.env = env;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<string> Test()
        {
            return "Return";
        }

        [HttpPost]
        public async Task<ActionResult<IList<UploadResult>>> PostFile(
            [FromForm] IEnumerable<IFormFile> files )
        {
            var maxAllowedFiles = 3;
            long maxFileSize = 1024 * 15;
            var filesProcessed = 0;
            var resourcePath = new Uri( $"{Request.Scheme}://{Request.Host}/" );
            List<UploadResult> uploadResults = new();

            foreach ( var file in files )
            {
                var uploadResult = new UploadResult();
                string trustedFileNameForFileStorage;
                var untrustedFileName = file.FileName;
                uploadResult.FileName = untrustedFileName;
                var trustedFileNameForDisplay =
                    WebUtility.HtmlEncode( untrustedFileName );

                if ( filesProcessed < maxAllowedFiles )
                {
                    if ( file.Length == 0 )
                    {
                        logger.LogInformation( "{FileName} length is 0 (Err: 1)",
                            trustedFileNameForDisplay );
                        uploadResult.ErrorCode = 1;
                    }
                    else
                    {
                        try
                        {
                            trustedFileNameForFileStorage = Path.GetRandomFileName();
                            var path = @"D:\BigFileUploadedApi.mkv";

                            await using FileStream fs = new( path, FileMode.Create );
                            await file.CopyToAsync( fs );

                            logger.LogInformation( "{FileName} saved at {Path}",
                                trustedFileNameForDisplay, path );
                            uploadResult.Uploaded = true;
                            uploadResult.StoredFileName = trustedFileNameForFileStorage;
                        }
                        catch ( IOException ex )
                        {
                            logger.LogError( "{FileName} error on upload (Err: 3): {Message}",
                                trustedFileNameForDisplay, ex.Message );
                            uploadResult.ErrorCode = 3;
                        }
                    }

                    filesProcessed++;
                }
                else
                {
                    logger.LogInformation( "{FileName} not uploaded because the " +
                        "request exceeded the allowed {Count} of files (Err: 4)",
                        trustedFileNameForDisplay, maxAllowedFiles );
                    uploadResult.ErrorCode = 4;
                }

                uploadResults.Add( uploadResult );
            }

            return new CreatedResult( resourcePath, uploadResults );
        }
    }
}