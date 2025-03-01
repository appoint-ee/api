using api.Controllers.Models;
using Microsoft.AspNetCore.JsonPatch;

namespace api.Services;

public interface IProfileService
{
    Task Create(CreateProfileRequest request);
    GetProfileDetailsResponse? GetDetails(string userName);
    Task<bool> Patch(long id,  JsonPatchDocument? patchDoc);
}