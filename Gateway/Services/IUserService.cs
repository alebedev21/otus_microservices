using Gateway.DTO.Income;

namespace Gateway.Services;

public interface IUserService
{
    Task Update(UserDemoUpdateRequest request);
}
