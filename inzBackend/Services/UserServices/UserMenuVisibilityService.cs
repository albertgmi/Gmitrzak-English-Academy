using inzBackend.Entities.Identity;
using inzBackend.Exceptions;
using inzBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace inzBackend.Services.UserServices
{
    public class UserMenuVisibilityService : IUserMenuVisibilityService
    {
        private readonly GmitrzakEnglishAcademyDbContext _dbContext;
        private readonly IUserContextService _userContextService;

        public UserMenuVisibilityService(GmitrzakEnglishAcademyDbContext dbContext, IUserContextService userContextService)
        {
            _dbContext = dbContext;
            _userContextService = userContextService;
        }

        public List<string> GetHiddenMenuItemsForUser(int userId)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
                throw new NotFoundException("User not found");

            return _dbContext.UserMenuVisibilities
                .Where(v => v.UserId == userId && !v.IsVisible)
                .Select(v => v.MenuItemKey)
                .ToList();
        }

        public List<string> GetMyHiddenMenuItems()
        {
            try
            {
                var currentUserId = _userContextService.GetUserId;
                if (!currentUserId.HasValue)
                    return new List<string>();

                return _dbContext.UserMenuVisibilities
                    .Where(v => v.UserId == currentUserId.Value && !v.IsVisible)
                    .Select(v => v.MenuItemKey)
                    .ToList();
            }
            catch
            {
                return new List<string>();
            }
        }

        public void UpdateUserMenuVisibility(int userId, List<string> hiddenMenuItemKeys)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
                throw new NotFoundException("User not found");

            var distinctHiddenKeys = (hiddenMenuItemKeys ?? new List<string>())
                .Where(k => !string.IsNullOrWhiteSpace(k))
                .Distinct()
                .ToList();

            var existingVisibilities = _dbContext.UserMenuVisibilities
                .IgnoreQueryFilters()
                .Where(v => v.UserId == userId)
                .ToList();

            // Remove entries that are no longer hidden
            var toRemove = existingVisibilities
                .Where(v => !distinctHiddenKeys.Contains(v.MenuItemKey))
                .ToList();
            if (toRemove.Count > 0)
            {
                _dbContext.UserMenuVisibilities.RemoveRange(toRemove);
            }

            // Add or update entries that are hidden
            foreach (var key in distinctHiddenKeys)
            {
                var existing = existingVisibilities.FirstOrDefault(v => v.MenuItemKey == key);
                if (existing != null)
                {
                    existing.IsVisible = false;
                    existing.IsDeleted = false;
                }
                else
                {
                    _dbContext.UserMenuVisibilities.Add(new UserMenuVisibility
                    {
                        UserId = userId,
                        MenuItemKey = key,
                        IsVisible = false
                    });
                }
            }

            _dbContext.SaveChanges();
        }
    }
}
