using JsonToPdfGenerator.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace JsonToPdfGenerator.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            string jsonFilePath = "InputFile/sample-json-input.json";
            FileContentResult pdfResult = JsonToPdf.ConvertJsonToPdf(jsonFilePath);

            // Set to download pdf file
            Response.Headers.Add("content-disposition", $"attachment; filename={pdfResult.FileDownloadName}");

            return File(pdfResult.FileContents, pdfResult.ContentType);
        }

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