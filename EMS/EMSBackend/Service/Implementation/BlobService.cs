using Azure.Storage.Blobs;
using EMSBackend.Service.Abstraction;

namespace EMSBackend.Service.Implementation
{
    public class BlobService : IBlobService
    {
        private readonly IConfiguration _configuration;

        public BlobService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<(string bloburl, string storedFileName)> UploadFileAsync(IFormFile file)
        {
            var conStr = _configuration["AzureBlobStorage:ConnectionString"];

            var containerName = _configuration["AzureBlobStorage:ContainerName"];

            BlobContainerClient blobContainer = new BlobContainerClient(conStr, containerName);

            await blobContainer.CreateIfNotExistsAsync();

            string storedFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

            BlobClient blobClient = blobContainer.GetBlobClient(storedFileName);

            using(var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, true);
            }

            return (blobClient.Uri.ToString(), storedFileName);
        }
    }
}
