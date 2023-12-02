namespace MobileWeb.Models.DTOs;

public class UserDTO
{
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public DateTime? Birthday { get; set; }
    public IFormFile? Avatar { get; set; }
    public string AvatarUrl { get; set; } = string.Empty;
}
