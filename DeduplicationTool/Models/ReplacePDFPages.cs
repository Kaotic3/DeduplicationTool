using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DeduplicationTool.Models
{
    public class ReplacePDFPages
    {
        public ReplacePDFPages()
        {

        }
        public async Task<byte[]> ReplacePaginateText(MemoryStream pdfFile, string placeHolder)
        {
            SHA256 sha256 = SHA256.Create();
            HashSet<string> pages = new HashSet<string>();
            List<int> pagesToCopy = new List<int>();
            List<string> duplicatePages = new List<string>();
            StringBuilder textBuilder = new StringBuilder();
            
            var outStream = new MemoryStream();

            using (var pdfIn = new PdfDocument(new PdfReader(pdfFile)))
            {
                using (var pdfOut = new PdfDocument(new PdfWriter(outStream)))
                {
                    for (int page = 1; page <= pdfIn.GetNumberOfPages(); page++)
                    {
                        textBuilder.Clear();
                        try
                        {

                            var strategy = new SimpleTextExtractionStrategy();
                            string pageContent = PdfTextExtractor.GetTextFromPage(pdfIn.GetPage(page), strategy);
                            pageContent = pageContent.Trim();
                            var pageToCompare = PDFAnalysis.TrimAllWithInplaceCharArray(pageContent);

                            byte[] bytes = Encoding.ASCII.GetBytes(pageToCompare);
                            var textHash = sha256.ComputeHash(bytes);
                            for (int i = 0; i < textHash.Length; i++)
                            {
                                textBuilder.Append(textHash[i].ToString("x2"));
                            }
                            duplicatePages.Add(textBuilder.ToString());
                            if (!pages.Contains(textBuilder.ToString()))
                            {
                                pdfIn.CopyPagesTo(page, page, pdfOut, page + 1);
                                pages.Add(textBuilder.ToString());
                            }
                            else
                            {
                                var dupePage = duplicatePages.IndexOf(textBuilder.ToString());

                                PdfPage newPage = pdfOut.AddNewPage();
                                PdfCanvas pdfCanvas = new PdfCanvas(newPage);
                                Rectangle rectangle = pdfIn.GetDefaultPageSize();
                                Div d = new Div();
                                var width = rectangle.GetWidth();
                                var height = rectangle.GetHeight();
                                d.SetWidth(width);
                                d.SetHeight(height);
                                d.SetVerticalAlignment(VerticalAlignment.MIDDLE);
                                pdfCanvas.Rectangle(rectangle);
                                pdfCanvas.Stroke();
                                Canvas canvas = new Canvas(pdfCanvas, rectangle);
                                PdfFont font = PdfFontFactory.CreateFont(StandardFonts.TIMES_ROMAN);
                                PdfFont bold = PdfFontFactory.CreateFont(StandardFonts.TIMES_BOLD);
                                string phrase = "";
                                if (placeHolder.Contains("Enter Placeholder Language here."))
                                {
                                    phrase = $"Duplicate of page {dupePage + 1}"; placeHolder.Replace("XX", $"{dupePage + 1}");
                                }
                                else
                                {
                                    phrase = placeHolder.Replace("XX", $"{dupePage + 1}");
                                }
                                Text newTitle2 = new Text($"{phrase}").SetFont(bold);
                                Paragraph p = new Paragraph().Add(newTitle2);
                                p.SetTextAlignment(TextAlignment.CENTER);
                                d.Add(p);
                                canvas.Add(d);
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                    for (int page = 1; page <= pdfOut.GetNumberOfPages(); page++)
                    {
                        Document document = new Document(pdfOut);
                        Rectangle rectangle = pdfOut.GetPage(page).GetPageSize();
                        var width = rectangle.GetWidth();
                        var middle = width / 2;

                        document.ShowTextAligned(new Paragraph(String.Format("Page " + page + " of " + pdfOut.GetNumberOfPages())), middle, 7, page, TextAlignment.CENTER, VerticalAlignment.BOTTOM, 0);
                    }
                }
            }
            return outStream.ToArray();
        }
        public async Task<byte[]> ReplacePaginateImages(MemoryStream pdfFile, string placeHolder)
        {
            SHA256 sha256 = SHA256.Create();
            HashSet<string> pages = new HashSet<string>();
            List<int> pagesToCopy = new List<int>();
            List<string> duplicatePages = new List<string>();
            StringBuilder textBuilder = new StringBuilder();
            
            var outStream = new MemoryStream();

            using (var pdfIn = new PdfDocument(new PdfReader(pdfFile)))
            {
                using (var pdfOut = new PdfDocument(new PdfWriter(outStream)))
                {
                    for (int page = 1; page <= pdfIn.GetNumberOfPages(); page++)
                    {
                        try
                        {
                            textBuilder.Clear();
                            var pageToCheck = pdfIn.GetPage(page);
                            var pageBytes = pageToCheck.GetContentBytes();
                            var imageHash = sha256.ComputeHash(pageBytes);

                            for (int i = 0; i < imageHash.Length; i++)
                            {
                                textBuilder.Append(imageHash[i].ToString("x2"));
                            }
                            duplicatePages.Add(textBuilder.ToString());
                            if (!pages.Contains(textBuilder.ToString()))
                            {
                                pdfIn.CopyPagesTo(page, page, pdfOut, page + 1);
                                pages.Add(textBuilder.ToString());
                            }
                            else
                            {
                                var dupePage = duplicatePages.IndexOf(textBuilder.ToString());

                                PdfPage newPage = pdfOut.AddNewPage();
                                PdfCanvas pdfCanvas = new PdfCanvas(newPage);
                                Rectangle rectangle = pdfIn.GetDefaultPageSize();
                                Div d = new Div();
                                var width = rectangle.GetWidth();
                                var height = rectangle.GetHeight();
                                d.SetWidth(width);
                                d.SetHeight(height);
                                d.SetVerticalAlignment(VerticalAlignment.MIDDLE);
                                pdfCanvas.Rectangle(rectangle);
                                pdfCanvas.Stroke();
                                Canvas canvas = new Canvas(pdfCanvas, rectangle);
                                PdfFont font = PdfFontFactory.CreateFont(StandardFonts.TIMES_ROMAN);
                                PdfFont bold = PdfFontFactory.CreateFont(StandardFonts.TIMES_BOLD);
                                var phrase = placeHolder.Replace("XX", $"{dupePage + 1}");
                                Text newTitle2 = new Text($"{phrase}").SetFont(bold);
                                Paragraph p = new Paragraph().Add(newTitle2);
                                p.SetTextAlignment(TextAlignment.CENTER);
                                d.Add(p);
                                canvas.Add(d);
                            }
                        }
                        catch (Exception ex)
                        {
                            
                        }
                    }
                    for (int page = 1; page <= pdfOut.GetNumberOfPages(); page++)
                    {
                        Document document = new Document(pdfOut);
                        Rectangle rectangle = pdfOut.GetPage(page).GetPageSize();
                        var width = rectangle.GetWidth();
                        var middle = width / 2;

                        document.ShowTextAligned(new Paragraph(String.Format("Page " + page + " of " + pdfOut.GetNumberOfPages())), middle, 7, page, TextAlignment.CENTER, VerticalAlignment.BOTTOM, 0);
                    }
                }
            }
            return outStream.ToArray();
        }
    }
}