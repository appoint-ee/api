using api.Services;
using api.Services.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ApiControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<GetUserResponse>>> GetAll()
    {
        var users = await _userService.GetAll();
        
        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GetUserResponse>> GetById([FromRoute] long id)
    {
        var user = await _userService.GetById(id);
        if (user == null) return NotFound();
        
        return Ok(user);
    }

    [HttpPost]
    public async Task<ActionResult<GetUserResponse>> Create([FromBody] CreateUserRequest userRequest)
    {
        var createdUser = await _userService.Create(userRequest);

        if (createdUser == null)
        {
            return Conflict();
        }
        
        return CreatedAtAction(nameof(GetById), 
            new
            {
                id = createdUser.Id
            }, createdUser);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute] long id, [FromBody] UpdateUserRequest request)
    {
        var updated = await _userService.Update(id, request);
        if (!updated) return NotFound();
        
        return NoContent();
    }
}