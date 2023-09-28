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
        #region Function to convert json to pdf
        public static FileContentResult ConvertJsonToPdf(string jsonInput, string fontName, int fontSize)
        {
            // Read JSON
            var jsonData = JsonConvert.DeserializeObject<Dictionary<string, JToken>>(jsonInput);

            // Create PDF document
            Document document = new Document();
            MemoryStream memoryStream = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);

            // Open document for writing
            document.Open();

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

            // Create a Unicode encoding for text conversion
            Encoding unicodeEncoding = Encoding.Unicode;

            foreach (var category in jsonData)
            {
                PdfPTable pdfTable = new PdfPTable(2);
                pdfTable.WidthPercentage = 80;                

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
                                arrayTable.WidthPercentage = 80;

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

            // Close document
            document.Close();

            // Prepare PDF result as respons
            byte[] pdfBytes = memoryStream.ToArray();
            return new FileContentResult(pdfBytes, "application/pdf")
            {
                FileDownloadName = "Data-output-json-to.pdf"
            };
        }
        #endregion
    }
}
