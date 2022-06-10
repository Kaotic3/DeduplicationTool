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
        public async Task<byte[]> MovePaginateText(MemoryStream pdfFile)
        {
            SHA256 sha256 = SHA256.Create();
            HashSet<string> pages = new HashSet<string>();
            List<int> pagesToCopy = new List<int>();
            List<string> duplicatePages = new List<string>();
            List<int> pageOrder = new List<int>();
            StringBuilder textBuilder = new StringBuilder();
            var outstream = new MemoryStream();
            int appendixPosition = 0;
            try
            {
                using (var pdfIn = new PdfDocument(new PdfReader(pdfFile)))
                {
                    using (var pdfOut = new PdfDocument(new PdfWriter(outstream)))
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
                                    pageOrder.Add(page);
                                }
                                else
                                {
                                    pagesToCopy.Add(page);
                                }
                            }
                            catch (Exception ex)
                            {
                                
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
                        Document document = new Document(pdfOut);

                        for (int page = 1; page <= pdfOut.GetNumberOfPages(); page++)
                        {
                            Rectangle rectangle2 = pdfOut.GetPage(page).GetPageSize();
                            var width2 = rectangle2.GetWidth();
                            var middle2 = width2 / 2;

                            document.ShowTextAligned(new Paragraph(String.Format("Page " + page + " of " + pdfOut.GetNumberOfPages())), middle2, 7, page, TextAlignment.CENTER, VerticalAlignment.BOTTOM, 0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                
            }
            
            return outstream.ToArray();
        }
        /// <summary>
        /// Moves duplicate pages based on the text on the page to the end of the document. Inserts an appendix page. Keeps a record of moved pages for repagination.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public async Task<byte[]> MoveRepaginateText(MemoryStream pdfFile)
        {
            SHA256 sha256 = SHA256.Create();
            HashSet<string> pages = new HashSet<string>();
            List<int> pagesToCopy = new List<int>();
            List<string> duplicatePages = new List<string>();
            List<int> pageOrder = new List<int>();
            StringBuilder textBuilder = new StringBuilder();
            var outstream = new MemoryStream();
            
            int appendixPosition = 0;
            try
            {
                using (var pdfIn = new PdfDocument(new PdfReader(pdfFile)))
                {
                    using (var pdfOut = new PdfDocument(new PdfWriter(outstream)))
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
                                    pageOrder.Add(page);
                                }
                                else
                                {
                                    pagesToCopy.Add(page);
                                }
                            }
                            catch (Exception ex)
                            {
                                
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
                        int pageCount = pdfOut.GetNumberOfPages();
                        List<int> pagesToRemove = new List<int>();
                        Document document = new Document(pdfOut);
                        int k = 1;
                        foreach (var pageInOrder in pageOrder)
                        {
                            if (pageInOrder == 99999999)
                            {
                                k++;
                            }
                            else
                            {
                                Rectangle rectangle2 = pdfOut.GetPage(k).GetPageSize();
                                var width2 = rectangle2.GetWidth();
                                var middle2 = width2 / 2;
                                document.ShowTextAligned(new Paragraph(String.Format("Page " + pageInOrder + " of " + pageCount)), middle2, 7, k, TextAlignment.CENTER, VerticalAlignment.BOTTOM, 0);
                                k++;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                
            }
            return outstream.ToArray();
        }
        /// <summary>
        /// Moves duplicate pages based on text on the page to the end of the document. Inserts an appendix page. Different process to repagination.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public async Task<byte[]> MovePaginateImages(MemoryStream pdfFile)
        {
            SHA256 sha256 = SHA256.Create();
            HashSet<string> pages = new HashSet<string>();
            List<int> pagesToCopy = new List<int>();
            List<string> duplicatePages = new List<string>();
            StringBuilder textBuilder = new StringBuilder();
            List<int> pageOrder = new List<int>();
            var outstream = new MemoryStream();
            
            try
            {
                using (var pdfIn = new PdfDocument(new PdfReader(pdfFile)))
                {
                    using (var pdfOut = new PdfDocument(new PdfWriter(outstream)))
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
                        Document document = new Document(pdfOut);

                        for (int page = 1; page <= pdfOut.GetNumberOfPages(); page++)
                        {
                            Rectangle rectangle2 = pdfOut.GetPage(page).GetPageSize();
                            var width2 = rectangle2.GetWidth();
                            var middle2 = width2 / 2;

                            document.ShowTextAligned(new Paragraph(String.Format("Page " + page + " of " + pdfOut.GetNumberOfPages())), middle2, 7, page, TextAlignment.CENTER, VerticalAlignment.BOTTOM, 0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                
            }
            return outstream.ToArray();
        }
        /// <summary>
        /// Moves duplicate pages based on images and text on the page to the end of the document. Inserts an appendix page. Keeps a record of moved pages for repagination.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public async Task<(MemoryStream, string)> MoveRepaginateImages(MemoryStream pdfFile)
        {
            SHA256 sha256 = SHA256.Create();
            HashSet<string> pages = new HashSet<string>();
            List<int> pagesToCopy = new List<int>();
            List<string> duplicatePages = new List<string>();
            StringBuilder textBuilder = new StringBuilder();
            List<int> pageOrder = new List<int>();
            var outstream = new MemoryStream();
            try
            {
                using (var pdfIn = new PdfDocument(new PdfReader(pdfFile)))
                {
                    using (var pdfOut = new PdfDocument(new PdfWriter(outstream)))
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
                
            }
            StringBuilder toSend = new StringBuilder();
            foreach (var page in pageOrder)
            {
                toSend.Append(page.ToString() + Environment.NewLine);
            }
            return (outstream, toSend.ToString());
        }
    }
}