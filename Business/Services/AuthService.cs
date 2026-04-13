using Core.Abstracts.IServices;
using Core.Concretes.DTOs;
using Core.Concretes.Entities;
using Microsoft.AspNetCore.Identity;
using Utilities.Responses;


namespace Business.Services
{
    public class AuthService : IAuthService
    {
       private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<ApplicationUserRole> roleManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        public AuthService(UserManager<ApplicationUser> userManager, RoleManager<ApplicationUserRole> roleManager, SignInManager<ApplicationUser> signInManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.signInManager = signInManager;
        }

        public Task<IResult> ChangePasswordAsync(ChangePasswordDto model)
        {
            throw new NotImplementedException();
        }

        public Task<IResult> ForgotPasswordAsync(string email)
        {
            throw new NotImplementedException();
        }

        public async Task<IResult> LoginAsync(LoginDto model)
        {
            try
            {
                var signInResult = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                if (signInResult.Succeeded)
                {
                    return Result.Success();
                }
                else if (signInResult.IsLockedOut)
                {
                    return Result.Fail(["Your account is locked out, Please contact your superior!"]);
                }
                else if (signInResult.IsNotAllowed)
                {
                    return Result.Fail(["Your account is not approved yet!"]);
                }
                else
                {
                    return Result.Fail(["Invalid login attempt!", "Password or email address not correct!"]);
                }
                

            }
            catch (Exception ex)
            {

                return Result.Fail(["System Error!",ex.Message]);
            }
        }

        public async Task LogoutAsync()
        {
            await signInManager.SignOutAsync();
        }

        public async Task<IResult> RegisterAsync(RegisterDto model)
        {   
            try
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                };

                var identityResult = await userManager.CreateAsync(user, model.Password);
                if (identityResult.Succeeded)
                {
                    if (!roleManager.Roles.Any(x => x.Name == "Manager"))
                    {
                        await roleManager.CreateAsync(new ApplicationUserRole { Name = "Manager" });
                    }
                    if (!roleManager.Roles.Any (x => x.Name == "SalesPerson"))
                    {
                        await roleManager.CreateAsync(new ApplicationUserRole { Name = "SalesPerson" });
                    }
                    await userManager.AddToRoleAsync(user, "SalesPerson");
                    return Result.Success();
                }
                else
                {
                    return Result.Fail(identityResult.Errors.Select(x => x.Description));
                }
            }
            catch (Exception ex)
            {
                return Result.Fail(["System Error!",  ex.Message]);
            }
        }

        public Task<IResult> ResetPasswordAsync(ResetPasswordDto model)
        {
            throw new NotImplementedException();
        }
    }
}
