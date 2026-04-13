using Core.Abstracts.Bases;
using Core.Concretes.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Concretes.Entities
{
    public class Opportunity : BaseEntity
    {
        [Required]
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Value { get; set; }

        [Required]
        public string Currency { get; set; } = null!;
        public DateTime? ExpectedCloseDate { get; set; }
        public DateTime? ActualCloseDate { get; set; }

        [ForeignKey(nameof(AssignedUser))]
        public string? AssignedUserId { get; set; }
        public virtual ApplicationUser? AssignedUser { get; set; }

        [Required, ForeignKey(nameof(Customer))]
        public string CustomerId { get; set; } = null!;
        public virtual Customer? Customer { get; set; }
        public OpportunityStage Stage { get; set; } = OpportunityStage.Qualification;
        public OpportunityStatus Status { get; set; } = OpportunityStatus.Open;
        public virtual ICollection<Activity> Activities { get; set; } = [];
    }
}
