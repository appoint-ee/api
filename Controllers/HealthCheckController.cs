using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class HealthCheckController : ApiControllerBase
{
    [Authorize]
    [HttpGet]
    public string GetHealthcheck()
    {
        return "OK";
    }
}