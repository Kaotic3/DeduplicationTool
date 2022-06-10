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
    public class MovePDFPages
    {
        public MovePDFPages()
        {

        }
        /// <summary>
        /// Moves duplicate pages based on the text on the page to the end of the document. Inserts an appendix page. Paginate is different to repaginate.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public async Task<string> MovePaginateText(IBrowserFile pdfFile)
        {
            SHA256 sha256 = SHA256.Create();
            HashSet<string> pages = new HashSet<string>();
            List<int> pagesToCopy = new List<int>();
            Stream stream = pdfFile.OpenReadStream();
            var filename = new MemoryStream();
            await stream.CopyToAsync(filename);
            filename.Position = 0;
            PdfReader pdfReader = new PdfReader(filename);
            PdfDocument pdfDoc = new PdfDocument(pdfReader);
            List<string> duplicatePages = new List<string>();
            List<int> pageOrder = new List<int>();
            StringBuilder textBuilder = new StringBuilder();

            var newTitle = pdfFile.Name.Replace(".pdf", "");
            var outputFileToUser = $"{newTitle} Moved on Text.pdf";
            var outputFile = System.IO.Path.Combine("./", outputFileToUser);
            int appendixPosition = 0;
            try
            {
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
                                    pageOrder.Add(page);
                                }
                                else
                                {
                                    pagesToCopy.Add(page);
                                }
                            }
                            catch (Exception ex)
                            {
                                return $"Unable to parse {filename} - Reason: {ex}";
                            }
                        }
                        var dupePage = duplicatePages.IndexOf(textBuilder.ToString());
                        PdfPage newPage = pdfOut.AddNewPage(pdfOut.GetNumberOfPages() + 1);
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
                        Text newTitle2 = new Text($"Appendix - Duplicate pages follow").SetFont(bold);
                        Paragraph p = new Paragraph().Add(newTitle2);
                        p.SetTextAlignment(TextAlignment.CENTER);
                        d.Add(p);
                        canvas.Add(d);
                        pageOrder.Add(99999999);
                        foreach (var copyPage in pagesToCopy)
                        {
                            for (int page = 1; page <= pdfIn.GetNumberOfPages(); page++)
                            {
                                if (page == copyPage)
                                {
                                    pdfIn.CopyPagesTo(page, page, pdfOut, pdfIn.GetNumberOfPages() + 1);
                                    pageOrder.Add(copyPage);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return $"Failed to read {filename} - read {ex.Message}";
            }
            StringBuilder toSend = new StringBuilder();
            toSend.Append(outputFile + Environment.NewLine);
            foreach (var page in pageOrder)
            {
                toSend.Append(page.ToString() + Environment.NewLine);
            }
            return toSend.ToString();
        }
        /// <summary>
        /// Moves duplicate pages based on the text on the page to the end of the document. Inserts an appendix page. Keeps a record of moved pages for repagination.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public async Task<string> MoveRepaginateText(IBrowserFile pdfFile)
        {
            SHA256 sha256 = SHA256.Create();
            HashSet<string> pages = new HashSet<string>();
            List<int> pagesToCopy = new List<int>();
            Stream stream = pdfFile.OpenReadStream();
            var filename = new MemoryStream();
            await stream.CopyToAsync(filename);
            filename.Position = 0;
            PdfReader pdfReader = new PdfReader(filename);
            PdfDocument pdfDoc = new PdfDocument(pdfReader);
            List<string> duplicatePages = new List<string>();
            List<int> pageOrder = new List<int>();
            StringBuilder textBuilder = new StringBuilder();

            var newTitle = pdfFile.Name.Replace(".pdf", "");
            var outputFileToUser = $"{newTitle} Moved on Text.pdf";
            var outputFile = System.IO.Path.Combine("./", outputFileToUser);
            int appendixPosition = 0;
            try
            {
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
                                    pageOrder.Add(page);
                                }
                                else
                                {
                                    pagesToCopy.Add(page);
                                }
                            }
                            catch (Exception ex)
                            {
                                return $"Unable to parse {filename} - Reason: {ex}";
                            }
                        }
                        var dupePage = duplicatePages.IndexOf(textBuilder.ToString());
                        PdfPage newPage = pdfOut.AddNewPage(pdfOut.GetNumberOfPages() + 1);
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
                        Text newTitle2 = new Text($"Appendix - Duplicate pages follow").SetFont(bold);
                        Paragraph p = new Paragraph().Add(newTitle2);
                        p.SetTextAlignment(TextAlignment.CENTER);
                        d.Add(p);
                        canvas.Add(d);
                        pageOrder.Add(99999999);
                        foreach (var copyPage in pagesToCopy)
                        {
                            for (int page = 1; page <= pdfIn.GetNumberOfPages(); page++)
                            {
                                if (page == copyPage)
                                {
                                    pdfIn.CopyPagesTo(page, page, pdfOut, pdfIn.GetNumberOfPages() + 1);
                                    pageOrder.Add(copyPage);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return $"Failed to read {filename} - read {ex.Message}";
            }
            StringBuilder toSend = new StringBuilder();
            toSend.Append(outputFile + Environment.NewLine);
            foreach (var page in pageOrder)
            {
                toSend.Append(page.ToString() + Environment.NewLine);
            }
            return toSend.ToString();
        }
        /// <summary>
        /// Moves duplicate pages based on text on the page to the end of the document. Inserts an appendix page. Different process to repagination.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public async Task<string> MovePaginateImages(IBrowserFile pdfFile)
        {
            SHA256 sha256 = SHA256.Create();
            HashSet<string> pages = new HashSet<string>();
            List<int> pagesToCopy = new List<int>();
            Stream stream = pdfFile.OpenReadStream();
            var filename = new MemoryStream();
            await stream.CopyToAsync(filename);
            filename.Position = 0;
            PdfReader pdfReader = new PdfReader(filename);
            PdfDocument pdfDoc = new PdfDocument(pdfReader);
            List<string> duplicatePages = new List<string>();
            StringBuilder textBuilder = new StringBuilder();
            List<int> pageOrder = new List<int>();


            var newTitle = pdfFile.Name.Replace(".pdf", "");
            var outputFileToUser = $"{newTitle} Moved on Images.pdf";
            var outputFile = System.IO.Path.Combine("./", outputFileToUser);
            try
            {
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
                                    pageOrder.Add(page);
                                }
                                else
                                {
                                    pagesToCopy.Add(page);

                                }
                            }
                            catch (Exception ex)
                            {
                                return $"Unable to parse {filename} - Reason: {ex}";
                            }
                        }
                        var dupePage = duplicatePages.IndexOf(textBuilder.ToString());
                        PdfPage newPage = pdfOut.AddNewPage(pdfOut.GetNumberOfPages() + 1);
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
                        Text newTitle2 = new Text($"Appendix - Duplicate pages follow").SetFont(bold);
                        Paragraph p = new Paragraph().Add(newTitle2);
                        p.SetTextAlignment(TextAlignment.CENTER);
                        d.Add(p);
                        canvas.Add(d);
                        pageOrder.Add(99999999);
                        foreach (var copyPage in pagesToCopy)
                        {
                            for (int page = 1; page <= pdfIn.GetNumberOfPages(); page++)
                            {
                                if (page == copyPage)
                                {
                                    pdfIn.CopyPagesTo(page, page, pdfOut, pdfIn.GetNumberOfPages() + 1);
                                    pageOrder.Add(copyPage);
                                }
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return $"Failed to read {filename} - read {ex.Message}";
            }
            StringBuilder toSend = new StringBuilder();
            toSend.Append(outputFile + Environment.NewLine);
            foreach (var page in pageOrder)
            {
                toSend.Append(page.ToString() + Environment.NewLine);
            }

            return toSend.ToString();
        }
        /// <summary>
        /// Moves duplicate pages based on images and text on the page to the end of the document. Inserts an appendix page. Keeps a record of moved pages for repagination.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public async Task<string> MoveRepaginateImages(IBrowserFile pdfFile)
        {
            SHA256 sha256 = SHA256.Create();
            HashSet<string> pages = new HashSet<string>();
            List<int> pagesToCopy = new List<int>();
            Stream stream = pdfFile.OpenReadStream();
            var filename = new MemoryStream();
            await stream.CopyToAsync(filename);
            filename.Position = 0;
            PdfReader pdfReader = new PdfReader(filename);
            PdfDocument pdfDoc = new PdfDocument(pdfReader);
            List<string> duplicatePages = new List<string>();
            StringBuilder textBuilder = new StringBuilder();
            List<int> pageOrder = new List<int>();


            var newTitle = pdfFile.Name.Replace(".pdf", "");
            var outputFileToUser = $"{newTitle} Moved on Images.pdf";
            var outputFile = System.IO.Path.Combine("./", outputFileToUser);
            try
            {
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
                                    pageOrder.Add(page);
                                }
                                else
                                {
                                    pagesToCopy.Add(page);

                                }
                            }
                            catch (Exception ex)
                            {
                                return $"Unable to parse {filename} - Reason: {ex}";
                            }
                        }
                        var dupePage = duplicatePages.IndexOf(textBuilder.ToString());
                        PdfPage newPage = pdfOut.AddNewPage(pdfOut.GetNumberOfPages() + 1);
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
                        Text newTitle2 = new Text($"Appendix - Duplicate pages follow").SetFont(bold);
                        Paragraph p = new Paragraph().Add(newTitle2);
                        p.SetTextAlignment(TextAlignment.CENTER);
                        d.Add(p);
                        canvas.Add(d);
                        pageOrder.Add(99999999);
                        foreach (var copyPage in pagesToCopy)
                        {
                            for (int page = 1; page <= pdfIn.GetNumberOfPages(); page++)
                            {
                                if (page == copyPage)
                                {
                                    pdfIn.CopyPagesTo(page, page, pdfOut, pdfIn.GetNumberOfPages() + 1);
                                    pageOrder.Add(copyPage);
                                }
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return $"Failed to read {filename} - read {ex.Message}";
            }
            StringBuilder toSend = new StringBuilder();
            toSend.Append(outputFile + Environment.NewLine);
            foreach (var page in pageOrder)
            {
                toSend.Append(page.ToString() + Environment.NewLine);
            }
            return toSend.ToString();
        }
    }
}