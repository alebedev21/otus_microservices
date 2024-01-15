using Gateway.DTO.Income;
using Gateway.DTO.Outcome;

namespace Gateway.Services
{
    public class AuthService(IHttpClientFactory httpClientFactory) : IAuthService
    {
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

        public async Task RegisterUser(RegistrationRequest request)
        {
            await Task.CompletedTask;


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
