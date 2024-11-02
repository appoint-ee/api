using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

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