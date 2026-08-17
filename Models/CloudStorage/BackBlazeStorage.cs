using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using MovieReservationSystem.Interfaces;

namespace MovieReservationSystem.Models.CloudStorage
{
    public class BackBlazeStorage : IStorageClient
    {
        private readonly IAmazonS3 _s3Client;
        
        public BackBlazeStorage(IAmazonS3 s3Client)
        {
            _s3Client = s3Client;
        }
        public async Task<bool> deleteFile(string objectKey, string bucketName)
        {
            try
            {
                var request = new DeleteObjectRequest
                {
                    BucketName = bucketName,
                    Key = objectKey
                };

                var response = await _s3Client.DeleteObjectAsync(request);
                return response.HttpStatusCode == System.Net.HttpStatusCode.NoContent;
            }
 
            catch (AmazonS3Exception)
            {
                return false;
            }
        }

        public async Task<string?> uploadFile(Stream fileStream, string objectKey, string contentTypem, string bucketName, CancellationToken cancellationToken = default)
        {
            try
            {
                if(fileStream.CanSeek && fileStream.Position != 0)
                {
                    fileStream.Position = 0;
                }

                var request = new PutObjectRequest
                {
                    BucketName = bucketName,
                    Key = objectKey,
                    InputStream = fileStream,
                    ContentType = contentTypem,
                    DisableDefaultChecksumValidation = true
                };

                var respone = await _s3Client.PutObjectAsync(request);
                return respone.ETag;
            }
            
            catch (AmazonS3Exception)
            {
                return null;
            }
            
        }

        public async Task<string> get_file_url(string bucket_name, string object_key)
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = bucket_name,
                Key = object_key,
                Expires = DateTime.UtcNow.AddHours(1)
            };

            var url = await _s3Client.GetPreSignedURLAsync(request);
            return url;
        }
    }
}