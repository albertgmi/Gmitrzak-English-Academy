using System.Security.Claims;

namespace inzBackend.Services.UserServices
{
    public class UserContextService : IUserContextService
    {
        private readonly IHttpContextAccessor _contextAccesor;
        public UserContextService(IHttpContextAccessor contextAccessor)
        {
            _contextAccesor = contextAccessor;
        }
        public ClaimsPrincipal User => _contextAccesor.HttpContext?.User;
        public int? GetUserId => User is null ? null : int.Parse(User.FindFirst(x => x.Type == ClaimTypes.NameIdentifier).Value);
        public string? GetUserName => User?.Identity?.Name;
    }
}
