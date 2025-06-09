using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using HotChocolate.Authorization;
using Microsoft.AspNetCore.Cors;
namespace Propagator.Api.Controllers
{

    [ApiController]
[Route("api/[controller]")]
[Authorize]
public class UploadController : ControllerBase
{

    private readonly string _bucketName;
    private readonly IAmazonS3 _s3Client;
    private const double TimeoutDuration = 12;
    
    

    public UploadController(IConfiguration configuration,IAuthService authService, IPostService postService)
    {
        _bucketName = configuration["Aws:BucketName"];
        var accessKey = configuration["Aws:AccessKey"];
        var secretKey = configuration["Aws:SecretKey"];
        _s3Client = new AmazonS3Client(accessKey, secretKey, RegionEndpoint.CACentral1);
        
    }

    [HttpPost("UploadFile")]

    public async Task<IActionResult> UploadFileAsync()
    {
        try
        {
            var file = Request.Form.Files["file"];
            if (file == null || file.Length == 0)
                return BadRequest(new UploadDto()
                {
                    IsSuccessFull = false,
                    ErrorMessage = "File is empty",
                });

            await using var stream = file.OpenReadStream();
            var fileKey = Guid.NewGuid() + System.IO.Path.GetExtension(file.FileName);
            var fileTransferUtility = new TransferUtility(_s3Client);

            await fileTransferUtility.UploadAsync(stream, _bucketName, fileKey);
            return Ok(new UploadDto()
            {
                IsSuccessFull = true,
                FileName = fileKey,
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new UploadDto()
            {
                IsSuccessFull = false,
                ErrorMessage = ex.Message,
            });
        }
    }
    
  
    [HttpPost("GetPresignedUrls")]
    public async Task<IActionResult> GetPresignedUrlsForChunksAsync()
    {
        var content = await new StreamReader(Request.Body).ReadToEndAsync();
        var requestBody = JsonConvert.DeserializeObject<ChunkModel>(content);

        try
        {
            if (requestBody!.ChunkNumbers == null || requestBody.ChunkNumbers.Count == 0)
            {
                return BadRequest("No chunk numbers provided.");
            }

            var initiateRequest = new InitiateMultipartUploadRequest
            {
                BucketName = _bucketName,
                Key = requestBody.Objectkey
            };

            var initiateResponse = _s3Client.InitiateMultipartUploadAsync(initiateRequest).Result;

            var presignedUrls = new List<string>();
            foreach (var chunkNumber in requestBody.ChunkNumbers)
            {
                // Use the same expiration for each chunk
                var presignedUrl = await GeneratePresignedURL(requestBody.Objectkey, chunkNumber, TimeoutDuration, initiateResponse.UploadId);
                presignedUrls.Add(presignedUrl);
            }

            return Ok(presignedUrls);
        }
        catch (Exception ex)
        {
            return BadRequest($"Internal server error: {ex.Message}");
        }
    }

    private async Task<string> GeneratePresignedURL(string objectKey, int chunkNumber, double duration, string uploadId)
    {
        try
        {
            AWSConfigsS3.UseSignatureVersion4 = true;
            var request = new GetPreSignedUrlRequest
            {
                BucketName = _bucketName,
                Key = objectKey,
                Expires = DateTime.UtcNow.AddHours(duration),
                Verb = HttpVerb.PUT,
                PartNumber = chunkNumber,
                // Adjust the key to differentiate between chunks, for example: sample_part1.txt, sample_part2.txt, etc.
                UploadId = uploadId
            };

            return await _s3Client.GetPreSignedURLAsync(request);
        }
        catch (AmazonS3Exception ex)
        {
            Console.WriteLine($"Error: '{ex.Message}'");
            throw;
        }
    }
        
    [HttpPost("UploadFileChunks")]
    public async Task<IActionResult> GetHLSPresignedUrlsForChunksAsync()
    {
        try
        {
            ChunkModel requestBody;
            IFormFile file;

            if (Request.ContentType.StartsWith("application/json"))
            {
                // Handle JSON request
                string jsonContent;
                using (var reader = new StreamReader(Request.Body))
                {
                    jsonContent = await reader.ReadToEndAsync();
                }
                requestBody = JsonConvert.DeserializeObject<ChunkModel>(jsonContent);
                file = null; // No file in JSON request
            }
            else if (Request.ContentType.StartsWith("multipart/form-data"))
            {
                // Handle multipart form data
                requestBody = GetChunkModelFromForm();
                file = Request.Form.Files.GetFile("file");
            }
            else
            {
                return BadRequest("Unsupported Content-Type. Use 'application/json' or 'multipart/form-data'.");
            }

            // Validate ChunkModel
            if (requestBody == null || requestBody.ChunkNumbers == null || requestBody.ChunkNumbers.Count == 0)
            {
                return BadRequest("No chunk numbers provided.");
            }

            // Handle file if present
            if (file != null && file.Length > 0)
            {
                string tempFilePath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), file.FileName);
                using (var stream = new FileStream(tempFilePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Process the file (e.g., convert to HLS)
    
            }

            // Generate presigned URLs for chunks
            var presignedUrls = requestBody.ChunkNumbers.Select(chunkNumber =>
                GeneratePresignedURL($"chunks/{chunkNumber}", 1)).ToList();

            return Ok(new { PresignedUrls = presignedUrls });
        }
        catch (Exception ex)
        {
            return BadRequest($"Internal server error: {ex.Message}");
        }
    }
    


    private ChunkModel GetChunkModelFromForm()
    {
        var chunkNumbersString = Request.Form["chunkNumbers"].ToString();
        var chunkNumbers = JsonConvert.DeserializeObject<List<int>>(chunkNumbersString);
        return new ChunkModel { ChunkNumbers = chunkNumbers };
    }

    private string GeneratePresignedURL(string objectKey, int timeoutHours)
    {
        try
        {
            AWSConfigsS3.UseSignatureVersion4 = true;
            var request = new GetPreSignedUrlRequest
            {
                BucketName = _bucketName,
                Key = objectKey,
                Expires = DateTime.UtcNow.AddHours(timeoutHours),
                Verb = HttpVerb.PUT
            };
            return _s3Client.GetPreSignedURL(request);
        }
        catch (AmazonS3Exception ex)
        {
            Console.WriteLine($"Error generating presigned URL: {ex.Message}");
            throw;
        }
    }

 
}
}