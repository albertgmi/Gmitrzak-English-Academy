namespace inzBackend.Services.UserServices
{
    public interface IUserMenuVisibilityService
    {
        List<string> GetHiddenMenuItemsForUser(int userId);
        void UpdateUserMenuVisibility(int userId, List<string> hiddenMenuItemKeys);
        List<string> GetMyHiddenMenuItems();
    }
}
