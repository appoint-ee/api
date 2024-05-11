using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[AllowAnonymous]
[ApiController]
[Route("[controller]")]
public class HealthCheckController : ApiControllerBase
{
    [HttpGet]
    public string GetHealthcheck()
    {
        return "OK";
    }
}