#nullable disable

using Core.Abstracts.IServices;
using Core.Concretes.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace UI.Web.Areas.Identity.Pages
{
    public class LoginModel(IAuthService service) : PageModel
    {
        [BindProperty]
        public LoginDto Input { get; set; }
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var result = await service.LoginAsync(Input);
                if (result.IsSuccess)
                {
                    return LocalRedirect("/");
                }

                foreach (var err in result.Messages)
                {
                    ModelState.AddModelError(string.Empty, err);
                }
            }
            return Page();
        }
    }
}
