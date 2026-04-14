using Core.Abstracts.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace UI.Web.Areas.Identity.Pages
{
    public class LogoutModel(IAuthService service) : PageModel
    {
        public IActionResult OnGet()
        {
            return NotFound();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await service.LogoutAsync();
            return LocalRedirect("/identity/login");
        }
    }
}
