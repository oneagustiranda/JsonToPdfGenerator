using System;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JsonToPdfGenerator
{
    public class JsonToPdf
    {
        #region Function to convert json to pdf
        public static FileContentResult ConvertJsonToPdf(string jsonFilePath)
        {
            // Read JSON from file
            string json = File.ReadAllText(jsonFilePath);
            var jsonData = JsonConvert.DeserializeObject<Dictionary<string, JToken>>(json);

            // Create PDF document
            Document document = new Document();
            MemoryStream memoryStream = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);

            // Open document for writing
            document.Open();

            foreach (var category in jsonData)
            {
                PdfPTable pdfTable = new PdfPTable(2);
                pdfTable.WidthPercentage = 80;                

                // Object data type
                if (category.Value is JObject categoryData)
                {
                    pdfTable.AddCell(category.Key);
                    pdfTable.AddCell(""); // Empty cell

                    foreach (var item in categoryData)
                    {
                        pdfTable.AddCell(item.Key);
                        pdfTable.AddCell(item.Value.ToString());
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
                                arrayTable.AddCell(category.Key);

                                // Add header column with key name
                                foreach (var arrayItemProperty in arrayItemObject.Properties())
                                {
                                    if (arrayItemObject.Properties().First() != arrayItemProperty)
                                    {
                                        arrayTable.AddCell(arrayItemProperty.Name);
                                    }                                    
                                }
                            }

                            // Add object values into table rows
                            foreach (var arrayItemProperty in arrayItemObject.Properties())
                            {
                                arrayTable.AddCell(arrayItemProperty.Value.ToString());
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
