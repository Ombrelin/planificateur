using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Planificateur.Core.Contracts;
using Planificateur.Core.Entities;
using Planificateur.Core.Exceptions;
using Planificateur.Core.Repositories;

namespace Planificateur.Core;

public class AuthenticationApplication
{
    
    private readonly IApplicationUsersRepository repository;
    private readonly string jwtSecret;

    public AuthenticationApplication(IApplicationUsersRepository repository, string jwtSecret)
    {
        this.repository = repository;
        this.jwtSecret = jwtSecret;
    }


    public async Task<RegisterResponse> Register(RegisterRequest request)
    {
        var applicationUser = new ApplicationUser(
            request.Username,
            request.Password
        );

        if (await UserAlreadyExists(request))
        {
            throw new ArgumentException("User already exists");
        }

        await this.repository.Insert(applicationUser);

        return new RegisterResponse(
            applicationUser.Id,
            applicationUser.Username
        );
    }

    private async Task<bool> UserAlreadyExists(RegisterRequest request)
    {
        return await this.repository.FindByUsername(request.Username) is not null;
    }

    public async Task<LoginResponse> Login(LoginRequest request)
    {
        ApplicationUser applicationUser = await repository.FindByUsername(request.Username) ??
                                          throw new NotFoundException("Not such user");
        if (!applicationUser.VerifyPassword(request.Password))
        {
            throw new ArgumentException("Passwords do not match");
        }


        var tokenHandler = new JwtSecurityTokenHandler();
        SecurityTokenDescriptor tokenDescriptor = BuildTokenDescriptor(applicationUser);
        SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

        return new LoginResponse(tokenHandler.WriteToken(token));
    }

    private SecurityTokenDescriptor BuildTokenDescriptor(ApplicationUser applicationUser)
    {
        return new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(new []
            {
                new Claim(ClaimTypes.NameIdentifier, applicationUser.Id.ToString()),
                new Claim(ClaimTypes.Name, applicationUser.Username),
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSecret)),
                    SecurityAlgorithms.HmacSha256)
        };
    }
}