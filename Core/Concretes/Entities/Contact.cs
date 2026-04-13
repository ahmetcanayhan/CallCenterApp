using Core.Abstracts.Bases;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Concretes.Entities
{
    public class Contact : BaseEntity
    {
        [Required]
        public string FirstName { get; set; } = null!;
        [Required]
        public string LastName { get; set; } = null!;
        public string? Title { get; set; }
        [Required]
        public string Email { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public bool IsPrimary { get; set; } = false;
        [Required, ForeignKey(nameof(Customer))]
        public string CustomerId { get; set; } = null!;
        public virtual Customer? Customer { get; set; }
    }
}
