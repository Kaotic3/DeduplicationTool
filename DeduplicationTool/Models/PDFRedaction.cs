using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.PdfCleanup;
using iText.PdfCleanup.Autosweep;
using Microsoft.AspNetCore.Components.Forms;
using System.Text.RegularExpressions;

namespace DeduplicationTool.Models
{
    public class PDFRedaction
    {
        public PDFRedaction()
        {

        }
        public async Task<byte[]> RedactText(MemoryStream pdfFile, string[] toRedact)
        {
            var outstream = new MemoryStream();
            PdfDocument pdf = new PdfDocument(new PdfReader(pdfFile), new PdfWriter(outstream));
            foreach (var item in toRedact)
            {
                ICleanupStrategy cleanupStrategy = new RegexBasedCleanupStrategy(new Regex($@"{item}", RegexOptions.IgnoreCase)).SetRedactionColor(ColorConstants.BLACK);
                PdfCleaner.AutoSweepCleanUp(pdf, cleanupStrategy);
            }
            pdf.Close();
            return outstream.ToArray();
        }
    }
}
