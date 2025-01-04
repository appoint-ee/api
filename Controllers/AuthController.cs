using api.Controllers.Models;
using api.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

// Relocate auto-generated endpoints to custom endpoints where needed
[ApiController]
[Route("api")]
public class AuthController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IUserService _userService;

    public AuthController(
        UserManager<IdentityUser> userManager, 
        IUserService userService)
    {
        _userManager = userManager;
        _userService = userService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] CreateUserRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = new IdentityUser { UserName = request.Email, Email = request.Email };
        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded) return BadRequest(result.Errors);
        
        await _userService.Create(request);
            
        return Ok();
    }
}