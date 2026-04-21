using Core.Abstracts.IServices;
using CsvHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniExcelLibs;
using System.Diagnostics;
using System.Globalization;
using System.Text.Json;
using System.Threading.Tasks;
using UI.Web.Models;

namespace UI.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ILeadService leadService;

        public HomeController(ILogger<HomeController> logger, ILeadService leadService)
        {
            _logger = logger;
            this.leadService = leadService;
        }

        public async Task<IActionResult> Index()
        {
            return View(await leadService.GetAllAsync(User));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadLeads(IFormFile file)
        {
            if (file == null)
            {
                return BadRequest();
            }
            var result = await leadService.ImportFromFileAsync(file);
            if (result.IsSuccess)
            {
                return RedirectToAction("Index");
            }
            return BadRequest(result);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ExportToExcel(string fileFormat)
        {
            var stream = new MemoryStream();

            var data = await leadService.GetAllAsync(User);
            stream.SaveAs(data);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "leeds.xlsx");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ExportToJson()
        {
            var data = await leadService.GetAllAsync(User);
            var options = new JsonSerializerOptions { WriteIndented = true };
            byte[] jsonBytes = JsonSerializer.SerializeToUtf8Bytes(data, options);

            return File(jsonBytes, "application/json", "leads.json");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ExportToCsv()
        {
            var data = await leadService.GetAllAsync(User);

            using var stream = new MemoryStream();
            using var writer = new StreamWriter(stream);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

            await csv.WriteRecordsAsync(data);
            await writer.FlushAsync();

            return File(stream.ToArray(), "text/csv", "leads.csv");
        }

        [Authorize(Roles = "Manager")]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
