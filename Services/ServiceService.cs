using api.Data;
using api.Data.Entities;
using api.Services.Dtos;
using Microsoft.EntityFrameworkCore;

namespace api.Services;

public class ServiceService : IServiceService
{
    private readonly DataContext _context;

    public ServiceService(DataContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }
    
    public async Task<IEnumerable<GetServiceResponse>> GetAll() 
    { 
        return await _context.Services
            .Select(s => new GetServiceResponse() 
            { 
                Id = s.Id,
                Name = s.Name, 
                DefaultDuration = s.DefaultDuration, 
                DefaultPrice = s.DefaultPrice
            })
            .ToListAsync();
    }
    
    public async Task<GetServiceResponse> GetById(long id) 
    { 
        var service = await _context.Services.FirstOrDefaultAsync(s => s.Id == id);
        if (service == null) return null; 
        
        return new GetServiceResponse
        { 
            Id = service.Id, 
            Name = service.Name, 
            DefaultDuration = service.DefaultDuration,
            DefaultPrice = service.DefaultPrice
        };
    }

    public async Task<CreateServiceResponse> Create(CreateServiceRequest serviceRequest) 
    { 
        var service = new Service 
        { 
            Name = serviceRequest.Name, 
            DefaultDuration = serviceRequest.DefaultDuration, 
            DefaultPrice = serviceRequest.DefaultPrice, 
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };
        
        _context.Services.Add(service); 
        await _context.SaveChangesAsync(); 
        return new CreateServiceResponse 
        { 
            Id = service.Id, 
            Name = service.Name, 
            DefaultDuration = service.DefaultDuration,
            DefaultPrice = service.DefaultPrice
        };
    }

    public async Task<bool> Update(long id, UpdateServiceRequest serviceRequest)
    {
        var existingService = await _context.Services.Where(s => s.Id == id).FirstOrDefaultAsync();
        if (existingService == null) return false;
           
        existingService.Name = serviceRequest.Name;
        existingService.DefaultDuration = serviceRequest.DefaultDuration;
        existingService.DefaultPrice = serviceRequest.DefaultPrice;
        existingService.UpdatedAt = DateTime.UtcNow;
          
        await _context.SaveChangesAsync();
        return true;
    }
}