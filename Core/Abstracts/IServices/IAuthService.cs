using Core.Concretes.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Responses;

namespace Core.Abstracts.IServices
{
    public interface IAuthService
    {
        Task<IResult> LoginAsync(LoginDto model);
        Task<IResult> RegisterAsync(RegisterDto model);
        Task LogoutAsync();
        Task<IResult> ChangePasswordAsync(ChangePasswordDto model);
        Task<IResult> ResetPasswordAsync(ResetPasswordDto model);
        Task<IResult> ForgotPasswordAsync(string email);
    }
}
