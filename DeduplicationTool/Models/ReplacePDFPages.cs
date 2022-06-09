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
        public static string ReplacePaginateText(string filename, string placeHolder)
        {
            SHA256 sha256 = SHA256.Create();
            var path = System.IO.Path.GetDirectoryName(filename);
            var title = System.IO.Path.GetFileName(filename);
            HashSet<string> pages = new HashSet<string>();
            List<int> pagesToCopy = new List<int>();

            PdfReader pdfReader = new PdfReader(filename);
            PdfDocument pdfDoc = new PdfDocument(pdfReader);
            List<string> duplicatePages = new List<string>();
            StringBuilder textBuilder = new StringBuilder();

            var newTitle = title.Replace(".pdf", "");
            var outputFileToUser = $"{newTitle} Text Placeholder.pdf";
            var outputFile = System.IO.Path.Combine(path, outputFileToUser);

            using (var pdfIn = new PdfDocument(new PdfReader(filename)))
            {
                using (var pdfOut = new PdfDocument(new PdfWriter(outputFile)))
                {
                    for (int page = 1; page <= pdfIn.GetNumberOfPages(); page++)
                    {
                        textBuilder.Clear();
                        try
                        {

                            var strategy = new SimpleTextExtractionStrategy();
                            string pageContent = PdfTextExtractor.GetTextFromPage(pdfDoc.GetPage(page), strategy);
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
                            return $"Unable to parse {filename} - Reason: {ex.Message}";
                        }
                    }
                }
            }
            return outputFile;
        }
        public static string ReplacePaginateImages(string filename, string placeHolder)
        {
            SHA256 sha256 = SHA256.Create();
            var path = System.IO.Path.GetDirectoryName(filename);
            var title = System.IO.Path.GetFileName(filename);
            HashSet<string> pages = new HashSet<string>();
            List<int> pagesToCopy = new List<int>();

            PdfReader pdfReader = new PdfReader(filename);
            PdfDocument pdfDoc = new PdfDocument(pdfReader);
            List<string> duplicatePages = new List<string>();
            StringBuilder textBuilder = new StringBuilder();

            var newTitle = title.Replace(".pdf", "");
            var outputFileToUser = $"{newTitle} Images Placeholder.pdf";
            var outputFile = System.IO.Path.Combine(path, outputFileToUser);

            using (var pdfIn = new PdfDocument(new PdfReader(filename)))
            {
                using (var pdfOut = new PdfDocument(new PdfWriter(outputFile)))
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
                            return $"Unable to parse {filename} - Reason: {ex.Message}";
                        }
                    }
                }
            }
            return outputFile;
        }
    }
}