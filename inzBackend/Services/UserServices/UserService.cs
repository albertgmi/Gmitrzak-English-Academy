using AutoMapper;
using inzBackend.Entities;
using inzBackend.Enums;
using inzBackend.Exceptions;
using inzBackend.Helpers;
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
        private readonly IMapper _mapper;
        private readonly IUserContextService _userContextService;
        public UserService(GmitrzakEnglishAcademyDbContext dbContext, 
            IPasswordHasher<AppUser> passwordHasher, AuthenticationSettings authenticationSettings, 
            IMapper mapper, IUserContextService userContextService)
        {
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
            _authenticationSettings = authenticationSettings;
            _mapper = mapper;
            _userContextService = userContextService;
        }

        public AppUserDto registerUser(RegisterUserRequest request)
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
            var profile = new Entities.Profile { UserId = newUser.Id, CurrentSemester = 1, EnglishLevel = Enums.EnglishLevel.Communicative };
            _dbContext.Profiles.Add(profile);
            _dbContext.SaveChanges();
            return _mapper.Map<AppUserDto>(newUser);
        }

        public string login(LoginUserRequest request)
        {
            var user = _dbContext
                .Users
                .FirstOrDefault(x => x.Username == request.Username);
            if (user is null)
                throw new BadRequestException("Invalid username or password");

            if (!user.IsActive)
                throw new BadRequestException("User is not active");

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
            var expires = PolandTime.DateTimeNow.AddDays(_authenticationSettings.JwtExpireDays);

            var token = new JwtSecurityToken(_authenticationSettings.JwtIssuer,
                _authenticationSettings.JwtIssuer,
                claims,
                expires: expires,
                signingCredentials: cred);
            var tokenhandler = new JwtSecurityTokenHandler();

            var today = PolandTime.Today;

            var alreadyLoggedToday = _dbContext.UserLoginLogs
                .Any(x => x.UserId == user.Id && x.LoginDate == today);

            if (!alreadyLoggedToday)
            {
                _dbContext.UserLoginLogs.Add(new UserLoginLog
                {
                    UserId = user.Id,
                    LoginDate = today,
                    LoginAt = PolandTime.Now
                });
                _dbContext.SaveChanges();
            }

            return tokenhandler.WriteToken(token);
        }

        public List<AppUserDto> getAllUsers()
        {
            return getUsers(true);
        }

        public List<AppUserDto> getAllInactiveUsers()
        {
            return getUsers(false);
        }

        public AppUserDto getUserById()
        {
            var userId = _userContextService.GetUserId;

            var user = _dbContext
                .Users
                .FirstOrDefault(u => u.Id == userId);

            return _mapper.Map<AppUserDto>(user);
        }

        public void updateUser(UpdateUserRequest request, int userId)
        {
            var user = _dbContext
                .Users
                .FirstOrDefault(x => x.Id == userId);
            if (user is null)
                throw new NotFoundException("User was not found");
            user.Username = request.Username;
            user.Email = request.Email;
            user.Role = request.Role;
            if (!string.IsNullOrWhiteSpace(request.Password))
            {
                var passwordHashed = _passwordHasher.HashPassword(user, request.Password);
                user.PasswordHash = passwordHashed;
            }
            user.IsActive = request.isActive;
            _dbContext.SaveChanges();
        }

        public void deleteUser(int userId)
        {
            var user = _dbContext
                .Users
                .FirstOrDefault(x => x.Id == userId);
            if (user is null)
                throw new NotFoundException("User was not found");
            _dbContext.Users.Remove(user);
            _dbContext.SaveChanges();
        }

        public void deleteManyUsers(List<int> userIds)
        {
            var usersToDelete = _dbContext.Users
                .Where(u => userIds.Contains(u.Id))
                .ToList();

            if (usersToDelete.Count != userIds.Count)
                throw new NotFoundException("Not all users had been found");

            _dbContext.Users.RemoveRange(usersToDelete);
            _dbContext.SaveChanges();
        }

        private List<AppUserDto> getUsers(bool isActive)
        {
            var users = _dbContext.Users
                .Where(u => u.IsActive == isActive)
                .Select(u => new AppUserDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    Email = u.Email,
                    Role = u.Role,
                    IsActive = u.IsActive,
                    AvatarUrl = u.Profile != null ? u.Profile.AvatarUrl : null
                })
                .ToList();

            return users;
        }
    }
}
