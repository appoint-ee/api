using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

public class ApiControllerBase : Controller
{
    protected string? GetAccessToken()
    {
        var authorizationHeader = HttpContext.Request.Headers["Authorization"].ToString();

        if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
        {
            return null;
        }

        return authorizationHeader["Bearer ".Length..];
    }
}