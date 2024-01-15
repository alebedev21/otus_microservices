using Gateway.ConfigOptions;
using Gateway.DTO.Income;
using Gateway.DTO.Outcome;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Net;

namespace Gateway.Services
{
    public class AuthService(IHttpClientFactory httpClientFactory, IOptions<ApiPointsOptions> options) : IAuthService
    {
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
        private readonly string _authServiceUrl = options.Value.AuthUrl!;

        public async Task RegisterUser(RegistrationRequest request)
        {
            using var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_authServiceUrl);

            JsonSerializerOptions jsonSerializerOptions = new()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new JsonStringEnumConverter() },
            };

            using var response = await client.PostAsync(
                    "/register",
                    JsonContent.Create(request, new MediaTypeHeaderValue("application/json"), jsonSerializerOptions));

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    return;

                case HttpStatusCode.Conflict:
                    throw new ArgumentOutOfRangeException();

                default:
                    throw new Exception($"Received unexpected response status code '{response.StatusCode}'");
            }
        }

        public async Task<TokensBundleResponse> LoginUser(RegistrationRequest request)
        {
            await Task.CompletedTask;

            return new TokensBundleResponse
            {
                AccessToken = "",
                RefreshToken = "",
            };
        }
    }
}
