using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using ServerApp.Models;

namespace ServerApp.Services;

public interface IAuthService
{ 
    Task<string> Authenticate(AuthModel model);

    Task<string> AuthenticateGithub(ExternalAuthModel model);
}