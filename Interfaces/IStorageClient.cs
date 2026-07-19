namespace MovieReservationSystem.Interfaces
{
    public interface IStorageClient
    {
        public Task<string?> uploadFile(FileStream fileStream, string objectKey, string contentTypem, string bucketName);
        public Task<bool> deleteFile(string objectKey, string bucketName);
    }
}