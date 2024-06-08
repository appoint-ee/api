using api.Controllers.Models;
using api.Services;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[ApiController]
[Route("[controller]")]
public class GoogleAuthController : ApiControllerBase
{
    private readonly IGoogleAuthService _googleAuthService;

    public GoogleAuthController(
        IGoogleAuthService googleAuthService)
    {
        _googleAuthService = googleAuthService ?? throw new ArgumentNullException(nameof(googleAuthService));
    }
    
    [HttpPost("store")]
    public IActionResult Store([FromBody] StoreGoogleAuthRequest request)
    {
        _googleAuthService.Store(request);

        return Ok();
    }
}