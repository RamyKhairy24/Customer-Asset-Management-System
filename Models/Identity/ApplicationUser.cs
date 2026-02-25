using Microsoft.AspNetCore.Identity;

namespace CustomerFluent.Models.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginDate { get; set; }
        public bool IsActive { get; set; } = true;
        
        // Navigation properties
        public string FullName => $"{FirstName} {LastName}";
    }

    public class ApplicationRole : IdentityRole
    {
        public string? Description { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}