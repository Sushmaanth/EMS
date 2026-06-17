using EMSFrontend.Api.Abstraction;
using EMSFrontend.Api.ApiException;
using EMSFrontend.Models;
using System.Net;
using System.Net.Http.Headers;

namespace EMSFrontend.Helpers
{
    public class ApiRequestHelper : IApiRequestHelper
    {
        private readonly IHttpContextAccessor _accessor;
        private readonly IAuthRequest _authRequest;
        public ApiRequestHelper(IHttpContextAccessor accessor,
        IAuthRequest authRequest)
        {
            _accessor = accessor;
            _authRequest = authRequest;
        }
        public async Task HandleErrorResponse(HttpResponseMessage response)
        {

            if (response.IsSuccessStatusCode)
            {
                return;
            }

            var error = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new UnauthorizedException("Session expired");
            }

            if ((int)response.StatusCode >= 500)
            {
                throw new Exception("Internal server error");
            }
            throw new ApiRequestException(error, (int)response.StatusCode);
        }

        public async Task<bool> RefreshTokenJwtAsync(HttpClient client)
        {
            try
            {
                var session = _accessor.HttpContext.Session;

                string jwtToken = session.GetString("JWToken");

                string refreshToken = session.GetString("RefreshToken");

                if (string.IsNullOrEmpty(jwtToken) || string.IsNullOrEmpty(refreshToken))
                {
                    return false;
                }
                var response = await _authRequest.RefreshTokenAsync(
                    new RefreshTokenViewModel
                    {
                        AccessToken = jwtToken,

                        RefreshToken =
                            refreshToken
                    });
                if (response == null)
                {
                    return false;
                }

                session.SetString("JWToken", response.Token);

                session.SetString("RefreshToken", response.RefreshToken);
                return true;
            }
            catch
            {
                _accessor.HttpContext.Session.Clear();

                return false;
            }
        }

        public void SetBearerToken(HttpClient client)
        {
            {
                try
                {
                    string token = _accessor.HttpContext.Session.GetString("JWToken");

                    if (!string.IsNullOrEmpty(token))
                    {
                        client.DefaultRequestHeaders.Authorization =
                            new AuthenticationHeaderValue(
                                "Bearer",
                                token);
                    }
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
            }
        }
    }
}