using api.Controllers.Models;
using api.Services;
using api.Services.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[ApiController]
[Route("[controller]")]
public class ServicesController : ApiControllerBase
{
    private readonly IServiceService _serviceService;

    public ServicesController(IServiceService serviceService)
    {
        _serviceService = serviceService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<GetServiceResponse>>> GetAll()
    {
        var services = await _serviceService.GetAll();
        
        return Ok(services);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GetServiceResponse>> GetById([FromRoute] long id)
    {
        var service = await _serviceService.GetById(id);
        if (service == null) return NotFound();
        
        return Ok(service);
    }

    [HttpPost]
    public async Task<ActionResult<ServiceDto>> Create([FromBody] CreateServiceRequest serviceDto)
    {
        var createdService = await _serviceService.Create(serviceDto);
        
        return CreatedAtAction(nameof(GetById), 
            new
            {
                id = createdService.Id
            }, 
            createdService);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute] long id, [FromBody] UpdateServiceRequest serviceDto)
    {
        var updated = await _serviceService.Update(id, serviceDto);
        if (!updated) return NotFound();
        
        return NoContent();
    }
}