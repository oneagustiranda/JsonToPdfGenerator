using System;
using System.IO;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static iTextSharp.text.pdf.AcroFields;

namespace JsonToPdfGenerator
{
    public class JsonToPdf
    {
        private static Font SetContentFont(string fontName, int fontSize)
        {
            // Set font and font size
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            // Set font and font size
            BaseFont bf = null;

            // Check if the font file exists
            string fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), $"{fontName}.ttf");
            if (File.Exists(fontPath))
            {
                bf = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            }
            else
            {
                // Use a default font if the specified font is not found
                bf = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, BaseFont.EMBEDDED);
            }

            Font font = new Font(bf, fontSize);
            return font;
        }

        private int ConvertCmToPoint(float cmValue)
        {
            float pointValue = cmValue * 28.3464567f;
            return (int)pointValue;
        }
        
        private static void SetMinimumMarginPdf(Document document, float leftMargin, float rightMargin, float topMargin, float bottomMargin)
        {
            JsonToPdf jsonToPdf = new JsonToPdf();

            // Minimum value for margin (size in point)
            int minLeftMargin = 20;
            int minRightMargin = 20;
            int minTopMargin = 30;
            int minBottomMargin = 50;

            // Margin value from user is cm
            int userLeftMargin = jsonToPdf.ConvertCmToPoint(leftMargin);
            int userRightMargin = jsonToPdf.ConvertCmToPoint(rightMargin);
            int userTopMargin = jsonToPdf.ConvertCmToPoint(topMargin);
            int userBottomMargin = jsonToPdf.ConvertCmToPoint(bottomMargin);

            // Margin value not less than minimum value
            int adjustedLeftMargin = Math.Max(userLeftMargin, minLeftMargin);
            int adjustedRightMargin = Math.Max(userRightMargin, minRightMargin);
            int adjustedTopMargin = Math.Max(userTopMargin, minTopMargin);
            int adjustedBottomMargin = Math.Max(userBottomMargin, minBottomMargin);

            // Set Margin to document
            document.SetMargins(adjustedLeftMargin, adjustedRightMargin, adjustedTopMargin, adjustedBottomMargin);
        }

        private static void ProcessDataToTable(JObject jsonObject, Document document, Font font)
        {
            // Create a Unicode encoding for text conversion
            Encoding unicodeEncoding = Encoding.Unicode;

            foreach (var category in jsonObject)
            {
                PdfPTable pdfTable = new PdfPTable(2);
                pdfTable.WidthPercentage = 100;

                // Object data type
                if (category.Value is JObject categoryData)
                {
                    pdfTable.AddCell(new Phrase(unicodeEncoding.GetString(unicodeEncoding.GetBytes(category.Key)), font));
                    pdfTable.AddCell(""); // Empty cell

                    foreach (var item in categoryData)
                    {
                        pdfTable.AddCell(new Phrase(unicodeEncoding.GetString(unicodeEncoding.GetBytes(item.Key)), font));
                        pdfTable.AddCell(new Phrase(unicodeEncoding.GetString(unicodeEncoding.GetBytes(item.Value.ToString())), font));
                    }
                }
                // Array data type
                else if (category.Value is JArray categoryArray)
                {
                    PdfPTable? arrayTable = null;

                    foreach (var arrayItem in categoryArray)
                    {
                        if (arrayItem is JObject arrayItemObject)
                        {
                            int numColumns = arrayItemObject.Properties().Count();

                            if (arrayTable == null)
                            {
                                // Create new table every time it encounters the first object in the array
                                arrayTable = new PdfPTable(numColumns);
                                arrayTable.WidthPercentage = 100;

                                // Add table name in first column                                
                                arrayTable.AddCell(new Phrase(unicodeEncoding.GetString(unicodeEncoding.GetBytes(category.Key)), font));

                                // Add header column with key name
                                foreach (var arrayItemProperty in arrayItemObject.Properties())
                                {
                                    if (arrayItemObject.Properties().First() != arrayItemProperty)
                                    {
                                        arrayTable.AddCell(new Phrase(unicodeEncoding.GetString(unicodeEncoding.GetBytes(arrayItemProperty.Name)), font));
                                    }
                                }
                            }

                            // Add object values into table rows
                            foreach (var arrayItemProperty in arrayItemObject.Properties())
                            {
                                arrayTable.AddCell(new Phrase(unicodeEncoding.GetString(unicodeEncoding.GetBytes(arrayItemProperty.Value.ToString())), font));
                            }
                        }
                    }

                    if (arrayTable != null)
                    {
                        document.Add(arrayTable); // Add array table to document
                    }
                }

                document.Add(pdfTable);

                // Add space area between table
                document.Add(new Paragraph(" "));
            }
        }

        #region Function to convert json to pdf
        public static string ConvertJsonToPdf(
            string jsonInput, IFormFile logoImage, string fontName, int fontSize, float leftMargin,
            float rightMargin, float topMargin, float bottomMargin, string headerText, string pdfPassword)
        {
            // Parse JSON
            var jsonData = JToken.Parse(jsonInput);

            // Create PDF document
            Document document = new Document();
            MemoryStream memoryStream = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);

            // Set password if have password value by user
            if (!string.IsNullOrEmpty(pdfPassword))
            {
                writer.SetEncryption(
                    Encoding.ASCII.GetBytes(pdfPassword), // Convert password to byte
                    Encoding.ASCII.GetBytes(pdfPassword), // Confirmation password
                    PdfWriter.ALLOW_PRINTING, PdfWriter.ENCRYPTION_AES_128);
            }

            // Check if a logo image was provided
            string logoPath = null;
            if (logoImage != null && logoImage.Length > 0)
            {
                // Save the logo image to a temporary location (adjust the path as needed)
                logoPath = Path.Combine(Path.GetTempPath(), logoImage.FileName);
                using (var stream = new FileStream(logoPath, FileMode.Create))
                {
                    logoImage.CopyTo(stream);
                }
            }

            Font font = SetContentFont(fontName, fontSize);

            // Add header and footer
            PdfHeaderFooter eventHelper = new PdfHeaderFooter(font, headerText, logoPath);
            writer.PageEvent = eventHelper;

            SetMinimumMarginPdf(document, leftMargin, rightMargin, topMargin, bottomMargin);

            // Open document for writing
            document.Open();
            document.Add(new Paragraph(" "));

            if (jsonData is JObject jsonObject)
            {
                ProcessDataToTable(jsonObject, document, font);
            }
            else if (jsonData is JArray jsonArray)
            {
                foreach (var arrayItem in jsonArray)
                {
                    if (arrayItem is JObject arrayItemObject)
                    {
                        ProcessDataToTable(arrayItemObject, document, font);
                    }
                }
            }

            // Close document
            document.Close();

            // Prepare PDF result as respons
            byte[] pdfBytes = memoryStream.ToArray();
            string pdfBase64 = Convert.ToBase64String(pdfBytes);

            // Remove logo image in temp folder
            if (!string.IsNullOrEmpty(logoPath) && System.IO.File.Exists(logoPath))
            {
                System.IO.File.Delete(logoPath);
            }

            return pdfBase64;
        }
        #endregion
    }
}
