using Azure;
using EMSFrontend.Api.Abstraction;
using EMSFrontend.Api.ApiException;
using EMSFrontend.Helpers;
using EMSFrontend.Models;
using EMSFrontend.Models.Leavemodels;
using System.Net;

namespace EMSFrontend.Api.Implementation
{
    public class LeaveApiRequest : ILeaveApiRequest
    {
        private readonly HttpClient _client;
        private readonly IHttpContextAccessor _accessor;
        private readonly IApiRequestHelper _apiRequestHelper;
        public LeaveApiRequest(HttpClient client, IHttpContextAccessor accessor, IApiRequestHelper apiRequestHelper)
        {
            _client = client;
            _accessor = accessor;
            _apiRequestHelper = apiRequestHelper;
        }
        public async Task<LeaveRequestViewModel> ApplyLeaveAsync(ApplyLeaveViewModel model)
        {
            _apiRequestHelper.SetBearerToken(_client);

            var response = await _client.PostAsJsonAsync("apply", model);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                bool refreshed = await _apiRequestHelper.RefreshTokenJwtAsync(_client);

                if (!refreshed)
                {
                    _accessor.HttpContext.Session.Clear();

                    throw new UnauthorizedException("Session expired");
                }

                _apiRequestHelper.SetBearerToken(_client);

                response = await _client.PostAsJsonAsync("apply", model);
            }

            await _apiRequestHelper.HandleErrorResponse(response);

            var result = await response.Content.ReadFromJsonAsync<ServiceResponseViewModel<LeaveRequestViewModel>>();

            return result.Data;
        }

        public async Task<ICollection<LeaveRequestViewModel>> GetMyLeavesAsync()
        {
            _apiRequestHelper.SetBearerToken(_client);

            var response = await _client.GetAsync("my-leaves");

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                bool refreshed = await _apiRequestHelper.RefreshTokenJwtAsync(_client);

                if (!refreshed)
                {
                    _accessor.HttpContext.Session.Clear();

                    throw new UnauthorizedException("Session expired");
                }
            }

            _apiRequestHelper.SetBearerToken(_client);

            response = await _client.GetAsync("my-leaves");

            await _apiRequestHelper.HandleErrorResponse(response);

            var result = await response.Content.ReadFromJsonAsync<ServiceResponseViewModel<ICollection<LeaveRequestViewModel>>>();

            return result.Data;
        }

        public async Task<IEnumerable<LeaveRequestViewModel>> GetTeamLeavesAsync()
        {
            _apiRequestHelper.SetBearerToken(_client);

            var response = await _client.GetAsync("team-leaves");

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                bool refreshed = await _apiRequestHelper.RefreshTokenJwtAsync(_client);

                if (!refreshed)
                {
                    _accessor.HttpContext.Session.Clear();

                    throw new UnauthorizedException("Session expired");
                }
            }

            _apiRequestHelper.SetBearerToken(_client);

            response = await _client.GetAsync("team-leaves");

            await _apiRequestHelper.HandleErrorResponse(response);

            var result = await response.Content.ReadFromJsonAsync<ServiceResponseViewModel<IEnumerable<LeaveRequestViewModel>>>();

            return result.Data;
        }

        public async Task<ReviewLeaveViewModel>ReviewLeaveAsync(ReviewLeaveViewModel model)
        {
            _apiRequestHelper.SetBearerToken(_client);

            var response = await _client.PutAsJsonAsync("review",model);

            if (response.StatusCode ==
                HttpStatusCode.Unauthorized)
            {
                bool refreshed = await _apiRequestHelper.RefreshTokenJwtAsync(_client);

                if (!refreshed)
                {
                    _accessor.HttpContext.Session.Clear();

                    throw new UnauthorizedException("Session expired");
                }

            }
            _apiRequestHelper.SetBearerToken(_client);

            response = await _client.PutAsJsonAsync("review",model);

            await _apiRequestHelper.HandleErrorResponse(response);

            var result = await response.Content.ReadFromJsonAsync<ServiceResponseViewModel<ReviewLeaveViewModel>>();

            return result.Data;
        }
    }
}
