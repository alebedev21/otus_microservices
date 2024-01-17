using Gateway.ConfigOptions;
using Gateway.DTO.Income;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;

namespace Gateway.Services;

public class UserService(IHttpClientFactory httpClientFactory, IOptions<ApiPointsOptions> options) : IUserService
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly string _demoServiceUrl = options.Value.DemoUrl!;

    public async Task Update(UserDemoUpdateRequest request)
    {
        using var demoClient = _httpClientFactory.CreateClient();
        demoClient.BaseAddress = new Uri(_demoServiceUrl);

        var response = await demoClient.PutAsync(
            "/user",
            JsonContent.Create(request, new MediaTypeHeaderValue("application/json")));

        response.EnsureSuccessStatusCode();
    }
}
