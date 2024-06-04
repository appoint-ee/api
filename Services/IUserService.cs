using api.Controllers.Models;

namespace api.Services;

public interface IUserService
{
    GetUserByUserNameResponse? Get(string userName);
}