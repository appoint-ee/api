using System.Security.Authentication;
using api.Controllers.Models;
using api.Data;
using api.Data.Entities;

namespace api.Services;

public class GoogleAuthService : IGoogleAuthService
{
    private readonly DataContext _dataContext;

    public GoogleAuthService(
        DataContext dataContext)
    {
        _dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
    }
    
    public void Store(StoreGoogleAuthRequest request)
    {
        var auth = _dataContext.GoogleAuths.SingleOrDefault(g => g.UserId == request.UserId);

        if (auth == null)
        {
            ValidateUser(request.UserId);
            ValidateRefreshToken(request.RefreshToken);

            auth = new GoogleAuth
            {
                UserId = request.UserId,
                AccessToken = request.AccessToken,
                ExpiresIn = request.ExpiresIn,
                RefreshToken = request.RefreshToken,
                Scope = request.Scope,
                TokenType = request.TokenType,
                IdToken = request.IdToken
            };
            
            _dataContext.GoogleAuths.Add(auth);
        }
        else
        {
            if (request.RefreshToken != null)
            {
                auth.RefreshToken = request.RefreshToken;
            }

            auth.AccessToken = request.AccessToken;
            auth.ExpiresIn = request.ExpiresIn;
            auth.Scope = request.Scope;
            auth.TokenType = request.TokenType;
            auth.IdToken = request.IdToken;
        }

        _dataContext.SaveChanges();
    }
    
    private void ValidateUser(long userId)
    {
        var userExist = _dataContext.Users.Any(u => u.Id == userId);
        if (!userExist)
        {
            throw new ArgumentException("Invalid user!");
        }
    }
    
    private void ValidateRefreshToken(string? refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            throw new AuthenticationException("Invalid refresh token!");
        }
    }
}