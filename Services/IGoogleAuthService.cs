using api.Controllers.Models;

namespace api.Services;

public interface IGoogleAuthService
{
    void Store(StoreGoogleAuthRequest request);
}