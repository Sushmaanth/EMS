using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Dtos;
using EMSBackend.Service.Abstraction;
using Microsoft.AspNetCore.StaticFiles;

namespace EMSBackend.Service.Implementation
{
    public class BlobService : IBlobService
    {
        private readonly IConfiguration _configuration;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName;
        public BlobService(IConfiguration configuration)
        {
            _blobServiceClient = new BlobServiceClient(configuration["AzureBlobStorage:ConnectionString"]
                ?? throw new Exception("Azure Blob connection string not configured"));

            _containerName = configuration["AzureBlobStorage:ContainerName"] 
                ?? throw new Exception("Azure Blob container name not configured");
        }

        public async Task<(string bloburl, string storedFileName)> UploadFileAsync(IFormFile file,
            string employeeName,int employeeId,string documentName)
        {
            BlobContainerClient blobContainer = _blobServiceClient.GetBlobContainerClient(_containerName);

            await blobContainer.CreateIfNotExistsAsync();

            string uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

            string safeDocumentName = string.Concat(documentName.Split(Path.GetInvalidFileNameChars()));

            safeDocumentName = safeDocumentName.Replace(" ", "_");

            string storedFileName =$"Sushmaanth/{employeeName}/{employeeId}/{safeDocumentName}/{uniqueFileName}";

            BlobClient blobClient = blobContainer.GetBlobClient(storedFileName);

            using(var stream = file.OpenReadStream())
            {
                var provider = new FileExtensionContentTypeProvider();

                if (!provider.TryGetContentType(file.FileName, out string? contentType))
                {
                    contentType = "application/octet-stream";
                }

                await blobClient.UploadAsync(stream, new BlobUploadOptions
                {
                    HttpHeaders = new BlobHttpHeaders
                    {
                        ContentType = contentType
                    }
                });
            }

            return (blobClient.Uri.ToString(), storedFileName);
        }

        public async Task DeleteFileAsync(string fileName)
        {
            BlobContainerClient container = _blobServiceClient.GetBlobContainerClient(_containerName);

            BlobClient blob =container.GetBlobClient(fileName);

            await blob.DeleteIfExistsAsync();
        }

        public string GenerateReadSasUrl(string fileName)
        {
            BlobContainerClient container =_blobServiceClient.GetBlobContainerClient(_containerName);

            BlobClient blob =container.GetBlobClient(fileName);

            BlobSasBuilder sasBuilder = new()
            {
                BlobContainerName = _containerName,

                BlobName = fileName,

                Resource = "b",

                ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(30)
            };

            sasBuilder.SetPermissions(BlobSasPermissions.Read);

            Uri sasUri = blob.GenerateSasUri(sasBuilder);

            return sasUri.ToString();
        }
    }
}
