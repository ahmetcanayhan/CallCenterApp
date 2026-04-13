using Core.Abstracts.Bases;
using Core.Concretes.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Concretes.Entities
{
    public class Lead : BaseEntity
    {
        [Required]
        public string Name { get; set; } = null!;

        [Required]
        public string Email { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public string? Notes { get; set; }

        [ForeignKey(nameof (ConvertedCustomer))]
        public string? ConvertedCustomerId { get; set; }
        public virtual Customer? ConvertedCustomer { get; set; }
        public DateTime? ConvertedAt { get; set; }

        [ForeignKey(nameof(AssignedUser))]
        public string? AssignedUserId { get; set; }
        public virtual ApplicationUser? AssignedUser { get; set; }
        public LeadSource Source { get; set; }
        public LeadStatus Status { get; set; } = LeadStatus.New;
        public virtual ICollection<Activity> Activities { get; set; } = [];

    }
}
