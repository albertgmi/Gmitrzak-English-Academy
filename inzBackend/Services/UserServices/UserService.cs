using inzBackend.Exceptions;
using inzBackend.Jwt;
using inzBackend.Models;
using inzBackend.Models.UserModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace inzBackend.Services.UserServices
{
    public class UserService : IUserService
    {
        private readonly GmitrzakEnglishAcademyDbContext _dbContext;
        private readonly IPasswordHasher<AppUser> _passwordHasher;
        private readonly AuthenticationSettings _authenticationSettings;
        public UserService(GmitrzakEnglishAcademyDbContext dbContext, IPasswordHasher<AppUser> passwordHasher, AuthenticationSettings authenticationSettings)
        {
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
            _authenticationSettings = authenticationSettings;
        }

        public AppUser registerUser(RegisterUserRequest request)
        {
            var newUser = new AppUser()
            {
                Username = request.Username,
                Email = request.Email,
                Role = request.Role,
                IsActive = request.IsActive
            };
            var passwordHashed = _passwordHasher.HashPassword(newUser, request.Password);
            newUser.PasswordHash = passwordHashed;
            _dbContext.Users.Add(newUser);
            _dbContext.SaveChanges();
            return newUser;
        }

        public string Login(LoginUserRequest request)
        {
            var user = _dbContext
                .Users
                .FirstOrDefault(x => x.Username == request.Username);
            if (user is null)
                throw new BadRequestException("Invalid username or password");

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
            if (result == PasswordVerificationResult.Failed)
                throw new BadRequestException("Invalid username or password");

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, $"{user.Username}"),
                new Claim(ClaimTypes.Role, $"{user.Role.ToString()}")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authenticationSettings.JwtKey));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(_authenticationSettings.JwtExpireDays);

            var token = new JwtSecurityToken(_authenticationSettings.JwtIssuer,
                _authenticationSettings.JwtIssuer,
                claims,
                expires: expires,
                signingCredentials: cred);
            var tokenhandler = new JwtSecurityTokenHandler();
            return tokenhandler.WriteToken(token);
        }
    }
}
