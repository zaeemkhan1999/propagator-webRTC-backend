
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Apsy.App.Propagator.Domain.Common.Inputs;
using Path = System.IO.Path;

namespace Apsy.App.Propagator.Application.Services
{
    public class FileUploadService : IFileUploadService
    {
        private readonly IConfiguration _configuration;
        private readonly string _bucketName;
        private readonly IAmazonS3 _s3Client;
        private const double TimeoutDuration = 12;
        private readonly string URL = "https://new-propagator-bucket-dev.s3.ca-central-1.amazonaws.com";
        public FileUploadService(IConfiguration configurations)
        {
            _configuration = configurations;
            _bucketName = _configuration["Aws:BucketName"];
            var accessKey = _configuration["Aws:AccessKey"];
            var secretKey = _configuration["Aws:SecretKey"];
            _s3Client = new AmazonS3Client(accessKey, secretKey, RegionEndpoint.CACentral1);
        }
        
        public FileUploadDto SavefileinlocalfolderProduct(ProductInput input)
        {
            var extension = Path.GetExtension(input.Images[0].FileName);
            var fileName = $"{Guid.NewGuid()}{extension}";
            var folderPath = Path.Combine("TempUploadFolder",fileName);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var filePath = Path.Combine(folderPath, fileName);

            try
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    input.Images[0].CopyTo(stream);
                }
            }
            catch (Exception ex)
            {
                filePath = ex.Message + ex.StackTrace;
            }

            return new FileUploadDto
            {
                Filename = fileName,
                Extension = extension,
                UploadPath = filePath,
                contenturl = $"{URL}/{fileName}"
            };
        }
        
        

        public async Task<string> FileUploaderAsync(FileUploadDto uploadDto)
        {
            var request = new PutObjectRequest
            {
                BucketName = _bucketName,
                Key = uploadDto.Filename,
                FilePath = uploadDto.UploadPath,
                AutoCloseStream = true
            };
            var response = await _s3Client.PutObjectAsync(request);
            string url = await GeneratePresignedURL(uploadDto.Filename);
            File.Delete(uploadDto.UploadPath);
            return URL + "/" + uploadDto.Filename;
        }
        public async Task<string> ThumbnailUploaderAsync(string Imagestring)
        {
            byte[] imageBytes = Convert.FromBase64String(Imagestring);
            var FileName = $"{Guid.NewGuid()}{".png"}";
            var filePath = Path.Combine("TempUploadFolder", FileName);
            await File.WriteAllBytesAsync(filePath, imageBytes);
            var request = new PutObjectRequest
            {
                BucketName = _bucketName,
                Key = FileName,
                FilePath = filePath,
                AutoCloseStream = true
            };
            var response = await _s3Client.PutObjectAsync(request);
            File.Delete(filePath);
            return URL + "/" + FileName;
        }
        private async Task<string> GeneratePresignedURL(string objectKey)
        {
            try
            {
                AWSConfigsS3.UseSignatureVersion4 = true;
                var request = new GetPreSignedUrlRequest
                {
                    BucketName = _bucketName,
                    Key = objectKey,
                    Expires = DateTime.UtcNow.AddHours(24),
                    Verb = HttpVerb.GET,
                };
                return await _s3Client.GetPreSignedURLAsync(request);
            }
            catch (AmazonS3Exception ex)
            {
                Console.WriteLine($"Error: '{ex.Message}'");
                throw;
            }
        }
    }
}
