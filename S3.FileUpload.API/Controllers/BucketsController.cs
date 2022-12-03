using Amazon.S3;
using Microsoft.AspNetCore.Mvc;

namespace S3.FileUpload.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BucketsController : ControllerBase
    {
        private readonly IAmazonS3 _s3Client;

        public BucketsController(IAmazonS3 s3Client)
        {
            _s3Client = s3Client;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateBucketAsync(string bucketName)
        {
            var bucketExists = await _s3Client.DoesS3BucketExistAsync(bucketName);
            
            if (bucketExists)
            {
                return BadRequest(new { Message = $"Bucket {bucketName} already exists." });
            }

            await _s3Client.PutBucketAsync(bucketName);

            return Ok(new { Message = $"Bucket {bucketName} created." });
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllBucketAsync()
        {
            var data = await _s3Client.ListBucketsAsync();
            var buckets = data.Buckets.Select(b => b.BucketName);

            return Ok(new { Buckets = buckets });
        }

        [HttpGet("delete")]
        public async Task<IActionResult> DeleteBucketAsync(string bucketName)
        {
            var bucketExists = await _s3Client.DoesS3BucketExistAsync(bucketName);

            if (!bucketExists)
            {
                return BadRequest(new { Message = $"Bucket {bucketName} does not exists." });
            }

            var objects = await _s3Client.ListObjectsAsync(bucketName);

            if (objects.S3Objects.Count > 0)
            {
                return BadRequest(new { Message = $"Bucket {bucketName} is not empty." });
            }

            await _s3Client.DeleteBucketAsync(bucketName);

            return NoContent();
        }
    }
}
