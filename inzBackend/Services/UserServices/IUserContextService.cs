using System.Security.Claims;

namespace inzBackend.Services.UserServices
{
    public interface IUserContextService
    {
        ClaimsPrincipal User { get; }
        int? GetUserId { get; }
        string? GetUserName { get; }
    }
}
