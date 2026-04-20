using Core.Concretes.Entities;
using Core.Concretes.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Concretes.DTOs
{
    public class LeadCreateDto
    {
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public string? Notes { get; set; }
        public LeadSource Source  { get; set; }
    }

    public class LeadListItemDto
    {
        public string Id { get; set; } = null!;  
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public string? Notes { get; set; }
        public string? ConvertedCustomerId { get; set; }
        public DateTime? ConvertedAt { get; set; }
        public string? AssignedUserId { get; set; }
        public string? AssignedUserName { get; set; }
        public LeadSource Source { get; set; }
        public LeadStatus Status { get; set; }
    }
}
