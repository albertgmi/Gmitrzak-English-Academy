namespace inzBackend.Models.UserModels
{
    public class UserMenuVisibilityDto
    {
        public string MenuItemKey { get; set; } = string.Empty;
        public bool IsVisible { get; set; } = true;
    }

    public class UpdateUserMenuVisibilityRequest
    {
        public List<string> HiddenMenuItemKeys { get; set; } = new List<string>();
    }
}
