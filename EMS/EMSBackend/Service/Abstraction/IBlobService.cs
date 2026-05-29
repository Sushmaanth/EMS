namespace EMSBackend.Service.Abstraction
{
    public interface IBlobService
    {
        //blob file upload
        Task<(string bloburl, string storedFileName)> UploadFileAsync(IFormFile file,
        string employeeName, int employeeId, string documentName);

        Task DeleteFileAsync(string fileName);

        string GenerateReadSasUrl(string fileName);
    }
}
