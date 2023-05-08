using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Planificateur.Core;
using Planificateur.Core.Contracts;

namespace Planificateur.Web.Controllers;

[AllowAnonymous]
[ApiController]
[Route("/api/authentication")]
public class AuthenticationController : ControllerBase
{
    private readonly AuthenticationApplication application;

    public AuthenticationController(AuthenticationApplication application)
    {
        this.application = application;
    }

    /// <summary>
    /// Register a new user.
    /// </summary>
    /// <param name="request"></param>
    /// <returns>New user data</returns>
    [HttpPost("register")]
    public async Task<ActionResult<RegisterResponse>> Register([FromBody] RegisterRequest request)
    {
        return Created("", await this.application.Register(request));
    }

    /// <summary>
    /// Login an user
    /// </summary>
    /// <param name="request"></param>
    /// <returns>JWT token for the user</returns>
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        return Ok(await this.application.Login(request));
    }
}