using api.Services.Dtos;

namespace api.Services;

public interface IServiceService
{
    Task<IEnumerable<GetServiceResponse>> GetAll();
    Task<GetServiceResponse> GetById(long id);
    Task<CreateServiceResponse> Create(CreateServiceRequest serviceRequest);
    Task<bool> Update(long id, UpdateServiceRequest serviceRequest);
}