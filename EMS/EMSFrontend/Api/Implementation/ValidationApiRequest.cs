using EMSFrontend.Api.Abstraction;
using System.Text.Json;

namespace EMSFrontend.Api.Implementation
{
    public class ValidationApiRequest: IValidationRequest
    {
        private readonly HttpClient _client;
        public ValidationApiRequest(HttpClient client, IHttpContextAccessor accessor, IAuthRequest authRequest)
        {
           _client = client;
           
        }
        //remote validation
        public async Task<bool> CheckEmailExistsAsync(string email)
        {
            var response =
                await _client.GetAsync($"email?email={email}");

            var content = await response.Content.ReadFromJsonAsync<bool>();

            //Console.WriteLine($"Status: {response.StatusCode}");
            //Console.WriteLine($"Content: {content}");

            return JsonSerializer.Deserialize<bool>(content);
        }

        public async Task<bool> CheckMobileExistsAsync(long mobile)
        {
            var response =
                await _client.GetAsync($"mobile?mobile={mobile}");

            return await response.Content.ReadFromJsonAsync<bool>();
        }
    }
}
