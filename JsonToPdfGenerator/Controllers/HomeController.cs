using JsonToPdfGenerator.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Text.Json;

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
            return View();
        }

        [HttpPost]
        public IActionResult ConvertJsonToPdf(string jsonInput, string fontType, int fontSize)
        {
            if (string.IsNullOrEmpty(jsonInput))
            {
                ViewBag.ErrorMessage = "JSON input is required";
                return View("Index");
            }

            FileContentResult pdfResult = JsonToPdf.ConvertJsonToPdf(jsonInput, fontType, fontSize);

            // Mengambil hasil PDF sebagai byte array
            byte[] pdfBytes = pdfResult.FileContents;

            // Mengonversi byte array menjadi string base64
            string base64Pdf = Convert.ToBase64String(pdfBytes);
            ViewBag.base64Pdf = base64Pdf;

            // Mengembalikan string base64 sebagai respons
            return Content(base64Pdf, "application/pdf");
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