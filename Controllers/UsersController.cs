using api.Controllers.Models;
using api.Services;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ApiControllerBase
{
    private readonly IUserService _userService;
    
    public UsersController(
        IUserService userService)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    }

    [HttpGet("{userName}")]
    public ActionResult<GetUserByUserNameResponse>? GetByUserName([FromRoute] string userName)
    {
        var user = _userService.Get(userName);

        if (user == null)
        {
            return NotFound();
        }

        return user;
    }
}