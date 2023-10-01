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
        public IActionResult ConvertJsonToPdf(string jsonInput, string fontName, int fontSize, float leftMargin, float rightMargin, float topMargin, float bottomMargin, string headerText)
        {
            try
            {
                if (string.IsNullOrEmpty(jsonInput))
                {
                    ViewBag.ErrorMessage = "JSON input is required";
                    return View("Index");
                }

                string pdfBase64 = JsonToPdf.ConvertJsonToPdf(jsonInput, fontName, fontSize, leftMargin, rightMargin, topMargin, bottomMargin, headerText);

                if (!string.IsNullOrEmpty(pdfBase64))
                {
                    // Send base64 string to view
                    ViewBag.PdfBase64 = pdfBase64;
                }
                else
                {
                    ViewBag.ErrorMessage = "Failed to generate PDF. Please check your JSON input.";
                }

                return View("Index");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
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