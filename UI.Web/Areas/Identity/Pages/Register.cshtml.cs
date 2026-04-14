#nullable disable

using Core.Abstracts.IServices;
using Core.Concretes.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace UI.Web.Areas.Identity.Pages
{
    public class RegisterModel(IAuthService service) : PageModel
    {
        [BindProperty]
        public RegisterDto Input { get; set; }
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var result = await service.RegisterAsync(Input);
                if (result.IsSuccess)
                {
                    return LocalRedirect("/identity/login");
                }
                else
                {
                    foreach (var err in result.Messages)
                    {
                        ModelState.AddModelError(string.Empty, err);
                    }
                }
            }
            return Page();
        }
    }
}
