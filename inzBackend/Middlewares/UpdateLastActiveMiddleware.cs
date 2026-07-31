using inzBackend.Helpers;
using inzBackend.Models;
using System.Security.Claims;

namespace inzBackend.Middlewares
{
    public class UpdateLastActiveMiddleware
    {
        private readonly RequestDelegate _next;

        public UpdateLastActiveMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, GmitrzakEnglishAcademyDbContext dbContext)
        {
            if (context.User.Identity?.IsAuthenticated == true)
            {
                var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (int.TryParse(userIdClaim, out var userId))
                {
                    var user = await dbContext.Users.FindAsync(userId);

                    if (user != null && (user.LastActiveAt == null || PolandTime.DateTimeNow - user.LastActiveAt > TimeSpan.FromSeconds(30)))
                    {
                        user.LastActiveAt = PolandTime.DateTimeNow;
                        await dbContext.SaveChangesAsync();
                    }
                }
            }

            await _next(context);
        }
    }
}
