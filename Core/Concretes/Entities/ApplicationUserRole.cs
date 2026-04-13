using Microsoft.AspNetCore.Identity;

namespace Core.Concretes.Entities
{
    public class ApplicationUserRole : IdentityRole
    {
        public string? Description { get; set; }

    }
}
