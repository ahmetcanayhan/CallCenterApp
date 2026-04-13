using Core.Abstracts.Bases;
using Core.Concretes.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Concretes.Entities
{
    public class Customer : BaseEntity
    {
        [Required]
        public string Name { get; set; } = null!;
        public string? TaxNumber { get; set; }
        public string? Industury { get; set; }
        public string? Adress { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        [ForeignKey(nameof(AssignedUser))]
        public string? AssignedUserId { get; set; }
        public virtual ApplicationUser? AssignedUser { get; set; }
        public bool IsPerson { get; set; }
        public CustomerStatus Status { get; set; } = CustomerStatus.Potential;
        public virtual ICollection<Contact> Contacts { get; set; } = [];
        public virtual ICollection<Opportunity> Opportunities { get; set; } = [];
        public virtual ICollection<Activity> Activities { get; set; } = [];
    }
}
