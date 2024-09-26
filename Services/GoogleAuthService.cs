using System.Security.Authentication;
using api.Controllers.Models;
using api.Data;
using api.Data.Entities;
using RestSharp;

namespace api.Services;

public class GoogleAuthService : IGoogleAuthService
{
    private readonly IConfiguration _configuration;
    private readonly DataContext _dataContext;

    public GoogleAuthService(
        IConfiguration configuration,
        DataContext dataContext)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
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

    public async Task<string> Update(long userId, string refreshToken)
    {
        var accessToken = string.Empty;
        var restClient = new RestClient();
        var request = new RestRequest("https://oauth2.googleapis.com/token");

        request.AddQueryParameter("client_id", _configuration["GoogleAPI:ClientId"]);
        request.AddQueryParameter("client_secret", _configuration["GoogleAPI:ClientSecret"]);
        request.AddQueryParameter("grant_type", "refresh_token");
        request.AddQueryParameter("refresh_token", refreshToken);

        var response = await restClient.ExecutePostAsync(request);
        
        if (response.StatusCode == System.Net.HttpStatusCode.OK)
        { 
            var apiResponse = System.Text.Json.JsonSerializer.Deserialize<StoreGoogleAuthRequest>(response.Content);

            if (apiResponse != null)
            {
                var googleAuth = _dataContext.GoogleAuths.SingleOrDefault(a => a.UserId == userId);

                if (googleAuth == null)
                {
                    googleAuth = new GoogleAuth()
                    {
                        AccessToken = apiResponse.AccessToken,
                        ExpiresIn = apiResponse.ExpiresIn,
                        IdToken = apiResponse.IdToken,
                        RefreshToken = refreshToken,
                        Scope = apiResponse.Scope,
                        TokenType = apiResponse.TokenType,
                        UserId = userId
                    };

                    _dataContext.GoogleAuths.Add(googleAuth);
                }
                else
                {
                    googleAuth.AccessToken = apiResponse.AccessToken;
                    googleAuth.ExpiresIn = apiResponse.ExpiresIn;
                }

                await _dataContext.SaveChangesAsync();
                accessToken = googleAuth.AccessToken;
            }
        }
        
        return accessToken;
    }

    public GetGoogleAuthResponse? Get(long userId)
    {
        return _dataContext.Users
            .Where(u => u.Id == userId)
            .Join(_dataContext.GoogleAuths,
                user => user.Id,
                auth => auth.UserId,
                (user, auth) => auth)
            .Select(a => new GetGoogleAuthResponse()
            {
                UserId = a.UserId,
                AccessToken = a.AccessToken,
                ExpiresIn = a.ExpiresIn,
                RefreshToken = a.RefreshToken,
                Scope = a.Scope,
                TokenType = a.TokenType,
                IdToken = a.IdToken
            })
            .FirstOrDefault();
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