namespace EMSBackend.Service.Abstraction
{
    public interface IBlobService
    {
        //blob file upload
        Task<(string bloburl, string storedFileName)> UploadFileAsync(IFormFile file);
    }
}
