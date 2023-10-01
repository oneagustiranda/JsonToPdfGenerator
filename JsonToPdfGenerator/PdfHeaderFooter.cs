using iTextSharp.text.pdf;
using iTextSharp.text;

namespace JsonToPdfGenerator
{
    public class PdfHeaderFooter : PdfPageEventHelper
    {
        private Font contentFont;
        private string headerText;
        public PdfHeaderFooter(Font contentFont, string headerText)
        {
            this.contentFont = contentFont;
            this.headerText = headerText;
        }

        public override void OnEndPage(PdfWriter writer, Document document)
        {
            // Add header
            PdfPTable headerTable = new PdfPTable(1);
            headerTable.TotalWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin;
            PdfPCell headerCell = new PdfPCell(new Phrase(headerText, contentFont));
            headerCell.Border = PdfPCell.NO_BORDER;
            headerTable.AddCell(headerCell);
            headerTable.WriteSelectedRows(0, -1, document.LeftMargin, document.PageSize.Height - document.TopMargin + headerTable.TotalHeight, writer.DirectContent);

            // Add footer
            PdfPTable footerTable = new PdfPTable(1);
            footerTable.TotalWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin;
            PdfPCell footerCell = new PdfPCell(new Phrase("Page " + writer.PageNumber, contentFont));
            footerCell.Border = PdfPCell.NO_BORDER;
            footerTable.AddCell(footerCell);
            footerTable.WriteSelectedRows(0, -1, document.LeftMargin, document.BottomMargin - footerTable.TotalHeight, writer.DirectContent);
        }
    }


}
