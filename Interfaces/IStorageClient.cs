namespace MovieReservationSystem.Interfaces
{
    public interface IStorageClient
    {
        public Task<string?> uploadFile(Stream fileStream, string objectKey, string contentTypem, string bucketName);
        public Task<bool> deleteFile(string objectKey, string bucketName);
        public Task<string> get_file_url(string bucket_name, string object_key);

    }
}