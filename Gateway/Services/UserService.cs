using Gateway.DTO.Income;
using Gateway.DTO.Outcome;

namespace Gateway.Services;

public class UserService() : IUserService
{
    public async Task<UserResponse> Add(UserAddRequest request)
    {
        await Task.CompletedTask;
        return new(Guid.NewGuid(), Guid.NewGuid(), "", "", "", "", "");
    }
}
