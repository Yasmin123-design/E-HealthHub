using Azure.Storage.Blobs;

namespace E_PharmaHub.Services
{
    public class FileStorageService : IFileStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;

        public FileStorageService(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
        }

        public async Task<string> SaveFileAsync(IFormFile file, string containerName)
        {
            if (file == null || file.Length == 0)
                return null;

            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

            await containerClient.CreateIfNotExistsAsync(Azure.Storage.Blobs.Models.PublicAccessType.Blob);

            var blobClient = containerClient.GetBlobClient(Guid.NewGuid() + Path.GetExtension(file.FileName));

            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream);
            }

            return blobClient.Uri.ToString();
        }


        public void DeleteFile(string fileUrl, string containerName)
        {
            if (string.IsNullOrEmpty(fileUrl)) return;

            var uri = new Uri(fileUrl);
            var blobName = Path.GetFileName(uri.LocalPath);

            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

            var blobClient = containerClient.GetBlobClient(blobName);
            blobClient.DeleteIfExists();
        }

    }


}
