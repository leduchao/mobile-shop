using Microsoft.AspNetCore.Identity;

namespace MobileWeb.Models.Entities
{
    // Add profile data for application users by adding properties to the User class
    public class User : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Address { get; set; }
        public DateTime? Birthday { get; set; }
        public DateTime? CreatedAt { get; set; } // ngay tao tai khoan
        public string? Avatar { get; set; }

        //public virtual List<Order> Orders { get; set; } = new();
    }
}