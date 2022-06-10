using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using iText.Layout;
using System.Threading.Tasks;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace DeduplicationTool.Models
{
    public class RemovePDFPages
    {
        public RemovePDFPages()
        {

        }
        public async Task<MemoryStream> RemovePaginateText(MemoryStream pdfFile)
        {
            SHA256 sha256 = SHA256.Create();
            HashSet<string> pages = new HashSet<string>();
            List<int> pagesToCopy = new List<int>();
            List<string> duplicatePages = new List<string>();
            StringBuilder pagesRemoved = new StringBuilder();
            List<int> pageOrder = new List<int>();
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
                                pagesRemoved.Append(page + Environment.NewLine);
                            }
                            else
                            {

                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
            }
            return outStream;
        }
        public async Task<MemoryStream> RemovePaginateImages(MemoryStream pdfFile)
        {
            SHA256 sha256 = SHA256.Create();
            
            HashSet<string> pages = new HashSet<string>();
            List<int> pagesToCopy = new List<int>();
            PdfReader pdfReader = new PdfReader(pdfFile);
            PdfDocument pdfDoc = new PdfDocument(pdfReader);
            List<string> duplicatePages = new List<string>();
            StringBuilder textBuilder = new StringBuilder();
            StringBuilder pagesRemoved = new StringBuilder();
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
                                pagesRemoved.Append(page + Environment.NewLine);
                            }
                            else
                            {

                            }
                        }
                        catch (Exception ex)
                        {
                            
                        }
                    }
                }
            }
            return outStream;
        }
        public async Task<(MemoryStream, string, int)> RemoveRepaginateText(MemoryStream pdfFile)
        {
            SHA256 sha256 = SHA256.Create();
            PDFAnalysis analysisModel = new PDFAnalysis();
            HashSet<string> pages = new HashSet<string>();
            List<string> duplicatePages = new List<string>();
            StringBuilder pagesRemoved = new StringBuilder();
            StringBuilder textBuilder = new StringBuilder();
            var outStream = new MemoryStream();
            int pageCount;
            outStream.Position = 0;
            using (var pdfIn = new PdfDocument(new PdfReader(pdfFile)))
            {
                using (var pdfOut = new PdfDocument(new PdfWriter(outStream)))
                {
                    Document document = new Document(pdfOut);
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
                                pagesRemoved.Append(page + Environment.NewLine);
                            }
                            else
                            {

                            }
                            

                        }
                        catch (Exception ex)
                        {
                            
                        }
                    }
                    pageCount = pdfOut.GetNumberOfPages();
                    var pageArray = pagesRemoved.ToString().Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                    List<string> pagesAsString = new List<string>(pageArray);
                    List<int> pagesToRemove = new List<int>();
                    foreach (var page in pagesAsString)
                    {
                        pagesToRemove.Add(int.Parse(page));
                    }
                    int k = 1;
                    foreach (var intPage in pagesToRemove)
                    {
                        if (intPage == 99999999)
                        {
                            k++;
                        }
                        else
                        {
                            Rectangle rectangle = pdfOut.GetPage(k).GetPageSize();
                            var width = rectangle.GetWidth();
                            var middle = width / 2;
                            document.ShowTextAligned(new Paragraph(String.Format("Page " + intPage + " of " + pageCount)), middle, 7, k, TextAlignment.CENTER, VerticalAlignment.BOTTOM, 0);
                            k++;
                        }
                    }
                    return (outStream, pagesRemoved.ToString(), pageCount);
                }
            }
            
        }
        public async Task<(MemoryStream, string)> RemoveRepaginateImages(MemoryStream pdfFile)
        {
            SHA256 sha256 = SHA256.Create();
            HashSet<string> pages = new HashSet<string>();
            List<string> duplicatePages = new List<string>();
            StringBuilder textBuilder = new StringBuilder();
            StringBuilder pagesRemoved = new StringBuilder();
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
                                pagesRemoved.Append(page + Environment.NewLine);
                            }
                            else
                            {

                            }
                        }
                        catch (Exception ex)
                        {
                            
                        }
                    }
                }
            }
            return (outStream, pagesRemoved.ToString());
        }
    }
}