using Core.Abstracts.Bases;
using Core.Concretes.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Concretes.Entities
{
    public class Activity : BaseEntity
    {
        [Required]
        public string Subject { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? CompletedAt { get; set; }
        public bool IsCompleted { get; set; } = false;

        [ForeignKey(nameof(AssignedUser))]
        public string? AssignedUserId { get; set; }
        public virtual ApplicationUser? AssignedUser { get; set; }

        [ForeignKey(nameof(RelatedCustomer))]
        public string? RelatedCustomerId { get; set; }
        public virtual Customer? RelatedCustomer { get; set; }

        [ForeignKey(nameof (RelatedLead))]
        public string? RelatedLeadId { get; set; }
        public virtual Lead? RelatedLead { get; set; }

        [ForeignKey(nameof(RelatedOpportunity))]
        public string? RelatedOpportunityId { get; set; }
        public virtual Opportunity? RelatedOpportunity { get; set; }
        public ActivityType Type { get; set; } 
    }
}
