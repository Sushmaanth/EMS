

namespace EMSFrontend.Api.ApiException
{
    public class ApiRequestException : Exception
    {
        public int StatusCode { get; }

        public ApiRequestException(string message, int statusCode): base(message)
        {
            StatusCode = statusCode;
        }
    }
}
