using System.Web;
using JsonToPdfGenerator.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;

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
        public IActionResult ConvertJsonToPdf(string jsonInput, IFormFile logoImage, string fontName, int fontSize, string pageSize, string pageOrientation,
            float leftMargin, float rightMargin, float topMargin, float bottomMargin, string headerText, string pdfPassword)
        {
            try
            {
                string pdfBase64 = JsonToPdf.ConvertJsonToPdf(jsonInput, logoImage, fontName, fontSize, pageSize, pageOrientation,
                    leftMargin, rightMargin, topMargin, bottomMargin, headerText, pdfPassword);

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