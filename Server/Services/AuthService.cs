using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using ServerApp.Models;

namespace ServerApp.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private IConfiguration _configuration;

    public AuthService(UserManager<IdentityUser> userManager, 
        SignInManager<IdentityUser> signInManager,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
    }

    private static Dictionary<string, string> tokens = new Dictionary<string, string>();

    public async Task<string> Authenticate(AuthModel model)
    {
        var user = await _userManager.FindByNameAsync(model.UserName);
        if (user is null) throw new AuthenticationException();
        var identityResult = await _signInManager
            .PasswordSignInAsync(user, model.Password, false, false);
        if (!identityResult.Succeeded)
            throw new AuthenticationException();

        var token = GenerateToken(user);
        return token;

    }

    public async Task<string> AuthenticateGithub(ExternalAuthModel model)
    {
        if (tokens.ContainsKey(model.AuthToken))
        {
            return tokens[model.AuthToken];
        }
        var clientId = _configuration["Authentication:github:client_id"];
        var clientSecret = _configuration["Authentication:github:client_secret"];
        var uri =
            $"https://github.com/login/oauth/access_token?client_id={clientId}&client_secret={clientSecret}&code={model.AuthToken}";

        var httpClient = new HttpClient();
        var httpRequest = new HttpRequestMessage();
        httpRequest.RequestUri = new Uri(uri);
        httpRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        httpRequest.Method = HttpMethod.Get;
        var response = await httpClient.SendAsync(httpRequest);
        var json = await response.Content.ReadAsStringAsync();
        var obj = JObject.Parse(json);
        if (obj.ContainsKey("error"))
        {
            throw new UnauthorizedAccessException();
        }

        var token = obj["access_token"].Value<string>();
        var type = obj["token_type"].Value<string>();
        var emailUri = $"https://api.github.com/user/emails?access_token={token}";
        httpRequest = new HttpRequestMessage();
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue(type, token);
        httpRequest.Headers.UserAgent.Add(new ProductInfoHeaderValue(new ProductHeaderValue("ASP.NET_APP")));
        httpRequest.Method = HttpMethod.Get;
        httpRequest.RequestUri = new Uri(emailUri);
        
        response = await httpClient.SendAsync(httpRequest);

        json = await response.Content.ReadAsStringAsync();
        var emails = JArray.Parse(json).ToArray().Select(x => (JObject)x)
            .Where(x => x.Property("primary").Value.ToString().ToLower() == true.ToString().ToLower())
            .ToArray();
        var emailObj = emails.FirstOrDefault();

        var email = ((JObject)emailObj).Property("email").Value.ToString();

        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
            throw new UnauthorizedAccessException();
        var jwt = GenerateToken(user);
        tokens[model.AuthToken] = jwt;
        return jwt;
    }

    public async Task<string> AuthenticateGoogle(ExternalAuthModel model)
    {
        var google = _configuration.GetSection("Authentication").GetSection("google");
        var clientId = google["client_id"];
        var clientSecret = google["client_secret"];

        var settings = new GoogleJsonWebSignature.ValidationSettings()
        {
            Audience = new List<string>() { clientId }
        };
        try
        {
            var payload = await GoogleJsonWebSignature.ValidateAsync(model.AuthToken, settings);
            var email = payload.Email;
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
            {
                throw new UnauthorizedAccessException();
            }

            return GenerateToken(user);
        }
        catch (Exception e)
        { 
            throw new UnauthorizedAccessException();
        }
    }
    
    private string GenerateToken(IdentityUser user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email)
        };
        
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(GetSecretKey()));

        var signInCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha512);

        var tokenOptions = new JwtSecurityToken(
            issuer: GetIssuer(),
            audience: GetAudience(),
            claims: claims,
            expires: DateTime.Now.AddHours(48),
            signingCredentials: signInCredentials);
        var token = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        return token;
    }
    
    
    private string GetSecretKey() => _configuration.GetSection("Jwt")["JwtSecretKey"];

    private string GetIssuer() => _configuration.GetSection("Jwt")["Issuer"];

    private string GetAudience() => _configuration.GetSection("Jwt")["Audience"];
}