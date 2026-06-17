namespace EMSFrontend.Helpers
{
    public interface IApiRequestHelper
    {
        void SetBearerToken(HttpClient client);
        Task HandleErrorResponse(HttpResponseMessage response);
        Task<bool> RefreshTokenJwtAsync(HttpClient client);
    }
}
