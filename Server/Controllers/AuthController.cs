using Microsoft.AspNetCore.Mvc;
using ServerApp.Models;
using ServerApp.Services;

namespace ServerApp.Controllers;

[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }
    
    [HttpPost("[action]")]
    public async Task<IActionResult> Authenticate([FromBody]AuthModel model)
    {
        try
        {
            var token = await _authService.Authenticate(model);
            return Ok(new { Token = token });
        }
        catch (Exception e)
        {
            return Unauthorized();
        }
    }

    [HttpPost("github/[action]")]
    public async Task<IActionResult> GithubAuth([FromBody]ExternalAuthModel model)
    {
        return Ok(new { Token = _authService.AuthenticateGithub(model) });
    }



}