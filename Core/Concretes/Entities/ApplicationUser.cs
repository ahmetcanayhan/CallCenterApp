using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Core.Concretes.Entities
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string FirstName { get; set; } = null!;

        [Required]
        public string LastName { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginDate { get; set; }
        public bool IsDeleted { get; set; } = false;

        public virtual ICollection<Customer> Customers { get; set; } = [];
        public virtual ICollection<Opportunity> Opportunities { get; set; } = [];
        public virtual ICollection<Activity> Activities { get; set; } = [];
        public virtual ICollection<Lead> Leads { get; set; } = [];
    }
}
