using AutoMapper;
using Core.Abstracts;
using Core.Abstracts.IRepositories;
using Core.Abstracts.IServices;
using Core.Concretes.DTOs;
using Core.Concretes.Entities;
using Core.Concretes.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Utilities.Helpers;
using Utilities.Responses;

namespace Business.Services
{
    public class LeadService(IUnitOfWork unitOfWork, IMapper mapper) : ILeadService
    {
        public async Task<Utilities.Responses.IResult> AddActivityAsync(ActivityType type, string leadId, ClaimsPrincipal user)
        {
            try
            {
                var lead = await unitOfWork.Leads.ReadByKeyAsync(leadId);
                if (lead != null && user != null)
                {
                    var activity = new Activity
                    {
                        Subject = $"{type}: {lead.Name}",
                        Type = type,
                        RelatedLeadId = leadId,
                        AssignedUserId = user.FindFirstValue(ClaimTypes.NameIdentifier),
                        DueDate = DateTime.Now,
                    };

                    await unitOfWork.Activities.CreateAsync(activity);
                    if (await unitOfWork.CommitAsync())
                    {
                        return Result.Success();
                    }
                    else
                    {
                        return Result.Fail(["Database operation failed!"]);
                    }
                }
                return Result.Fail(["Lead or User not found!"]);
            }
            catch (Exception ex)
            {

                return Result.Fail([ex.Message]);
            }
        }

        public async Task<Utilities.Responses.IResult> CreateAsync(LeadCreateDto lead)
        {
            try
            {
                var data = mapper.Map<Lead>(lead);
                await unitOfWork.Leads.CreateAsync(data);
                await unitOfWork.CommitAsync();
                return Result.Success();
            }
            catch (Exception ex)
            {

                return Result.Fail([ex.Message]);
            }
        }

        public async Task<IEnumerable<LeadListItemDto>> GetAllAsync(ClaimsPrincipal user)
        {
            if (user.IsInRole("Manager"))
            {
                var leads = await unitOfWork.Leads.ReadManyAsync(null, "Activities", "ConvertedCustomer", "AssignedUser");
                return mapper.Map<IEnumerable<LeadListItemDto>>(leads);
            }
            else
            {
                var leads = await unitOfWork.Leads.ReadManyAsync(x => x.AssignedUserId == user.FindFirstValue(ClaimTypes.NameIdentifier) || x.AssignedUserId == null, "Activities", "ConvertedCustomer", "AssignedUser");
                return mapper.Map<IEnumerable<LeadListItemDto>>(leads);
            }
        }

        public async Task<Utilities.Responses.IResult> ImportFromFileAsync(IFormFile file)
        {
            try
            {
                using var stream = file.OpenReadStream();
                string ext = Path.GetExtension(file.FileName);

                var result = ext switch
                {
                    ".csv" => await DataImporters.ImportCsvAsync<LeadCreateDto>(stream),
                    ".json" => await DataImporters.ImportJsonAsync<LeadCreateDto>(stream),
                    ".xlsx" => await DataImporters.ImportExcelAsync<LeadCreateDto>(stream),
                    ".xls" => await DataImporters.ImportExcelAsync<LeadCreateDto>(stream),
                    _ => null
                };

                if (result == null)
                {
                    return Result.Fail(["Only .csv, .json, .xlsx and .xls files accepted!"]);
                }

                var importedLeads = mapper.Map<IEnumerable<Lead>>(result);
                await unitOfWork.Leads.CreateManyAsync(importedLeads);
                await unitOfWork.CommitAsync();
                return Result.Success();
            }
            catch (Exception ex)
            {

                return Result.Fail([ex.Message]);
            }
        }

        public async Task<Utilities.Responses.IResult> PickLeadAsync(string leadId, ClaimsPrincipal user)
        {
            try
            {
                var lead = await unitOfWork.Leads.ReadByKeyAsync(leadId);
                if (lead == null)
                {
                    return Result.Fail(["Lead not found!"]);
                }
                lead.AssignedUserId = user.FindFirstValue(ClaimTypes.NameIdentifier);
                await unitOfWork.Leads.UpdateAsync(lead);
                if(await unitOfWork.CommitAsync())
                {
                    return Result.Success();
                }
                else
                {
                    return Result.Fail(["Lead Repository", "Database operation failed!"]);
                }
            }
            catch (Exception ex)
            {

                return Result.Fail([ex.Message]);
            }
        }
    }
}
