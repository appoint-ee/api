using api.Controllers.Models;
using api.Data;

namespace api.Services;

public class UserService : IUserService
{
    private readonly DataContext _context;

    public UserService(DataContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }
    
    public GetUserByUserNameResponse? Get(string userName)
    {
        return _context.Users
            .Where(u => u.UserName == userName)
            .Select(u => new GetUserByUserNameResponse()
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    EmailAddress = u.EmailAddress,
                    PhotoUrl = u.PhotoUrl,
                    Status = u.Status,
                    Address = u.Address,
                    CountryCode = u.CountryCode,
                    LangCode = u.LangCode,
                    PreferredTimeZone = u.PreferredTimeZone
                })
            .FirstOrDefault();
    }
}