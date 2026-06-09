namespace EMSFrontend.Api.Abstraction
{
    public interface IValidationRequest
    {
        //Remote Validation
        Task<bool> CheckEmailExistsAsync(string email);
        Task<bool> CheckMobileExistsAsync(long mobile);

        Task<bool> CheckEmployeeCodeExistsAsync(string employeeCode);
    }
}
