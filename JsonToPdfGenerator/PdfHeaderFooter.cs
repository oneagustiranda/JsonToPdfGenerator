using iTextSharp.text.pdf;
using iTextSharp.text;

namespace JsonToPdfGenerator
{
    public class PdfHeaderFooter : PdfPageEventHelper
    {
        private Font contentFont;
        private string headerText;
        private string logoPath;
        public PdfHeaderFooter(Font contentFont, string headerText, string logoPath)
        {
            this.contentFont = contentFont;
            this.headerText = headerText;
            this.logoPath = logoPath;
        }

        public override void OnEndPage(PdfWriter writer, Document document)
        {
            // Add header
            PdfPTable headerTable = new PdfPTable(1);
            headerTable.TotalWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin;

            string imagePath = logoPath;
            if (File.Exists(imagePath))
            {
                Image image = Image.GetInstance(imagePath);
                image.ScaleToFit(100f, 100f); // Image size
                PdfPCell imageCell = new PdfPCell(image);
                imageCell.Border = PdfPCell.NO_BORDER;
                headerTable.AddCell(imageCell);
            }

            Font boldFont = new Font(contentFont.BaseFont, contentFont.Size, Font.BOLD);
            PdfPCell textCell = new PdfPCell(new Phrase(headerText, boldFont));
            textCell.Border = PdfPCell.NO_BORDER;
            textCell.HorizontalAlignment = Element.ALIGN_CENTER;
            headerTable.AddCell(textCell);

            headerTable.WriteSelectedRows(0, -1, document.LeftMargin, document.PageSize.Height - document.TopMargin + headerTable.TotalHeight, writer.DirectContent);


            // Add footer
            PdfPTable footerTable = new PdfPTable(1);
            footerTable.TotalWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin;
            PdfPCell footerCell = new PdfPCell(new Phrase("Page " + writer.PageNumber, contentFont));
            footerCell.Border = PdfPCell.NO_BORDER;
            footerCell.HorizontalAlignment = Element.ALIGN_RIGHT;
            footerTable.AddCell(footerCell);
            footerTable.WriteSelectedRows(0, -1, document.LeftMargin, document.BottomMargin - footerTable.TotalHeight, writer.DirectContent);

        }
    }


}
