using Core.Concretes.DTOs;
using Core.Concretes.Enums;
using System.Security.Claims;
using Utilities.Responses;

namespace Core.Abstracts.IServices
{
    public interface ILeadService
    {
        Task<IEnumerable<LeadListItemDto>> GetAllAsync(ClaimsPrincipal user);
        Task<IResult> CreateAsync(LeadCreateDto lead);
        Task<IResult> ImportFromFileAsync(Microsoft.AspNetCore.Http.IFormFile file);
        Task<IResult> PickLeadAsync(string leadId, ClaimsPrincipal user);
        Task<IResult> AddActivityAsync(ActivityType type, string leadId, ClaimsPrincipal user);
    }
}
