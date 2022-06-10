using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DeduplicationTool.Models
{
    public class RemovePDFPages
    {
        public RemovePDFPages()
        {

        }
        public async Task<string> RemovePaginateText(IBrowserFile pdfFile)
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
            StringBuilder pagesRemoved = new StringBuilder();
            List<int> pageOrder = new List<int>();
            StringBuilder textBuilder = new StringBuilder();

            var newTitle = pdfFile.Name.Replace(".pdf", "");
            var outputFileToUser = $"{newTitle} Removed on Text.pdf";
            var outputFile = System.IO.Path.Combine("./", outputFileToUser);
            pagesRemoved.Append(outputFile + Environment.NewLine);

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
                                pagesRemoved.Append(page + Environment.NewLine);
                            }
                            else
                            {

                            }
                        }
                        catch (Exception ex)
                        {
                            return $"Unable to parse {filename} - Reason: {ex}";
                        }
                    }
                }
            }
            return pagesRemoved.ToString();
        }
        public async Task<string> RemovePaginateImages(IBrowserFile pdfFile)
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
            StringBuilder pagesRemoved = new StringBuilder();

            var newTitle = pdfFile.Name.Replace(".pdf", "");
            var outputFileToUser = $"{newTitle} Removed on Images.pdf";
            var outputFile = System.IO.Path.Combine("./", outputFileToUser);
            pagesRemoved.Append(outputFile + Environment.NewLine);
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
                                pagesRemoved.Append(page + Environment.NewLine);
                            }
                            else
                            {

                            }
                        }
                        catch (Exception ex)
                        {
                            return $"Unable to parse {filename} - Reason: {ex}";
                        }
                    }
                }
            }
            return pagesRemoved.ToString();
        }
        public async Task<string> RemoveRepaginateText(IBrowserFile pdfFile)
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
            StringBuilder pagesRemoved = new StringBuilder();
            List<int> pageOrder = new List<int>();
            StringBuilder textBuilder = new StringBuilder();

            var newTitle = pdfFile.Name.Replace(".pdf", "");
            var outputFileToUser = $"{newTitle} Removed on Text.pdf";
            var outputFile = System.IO.Path.Combine("./", outputFileToUser);
            pagesRemoved.Append(outputFile + Environment.NewLine);

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
                                pagesRemoved.Append(page + Environment.NewLine);
                            }
                            else
                            {

                            }
                        }
                        catch (Exception ex)
                        {
                            return $"Unable to parse {filename} - Reason: {ex}";
                        }
                    }
                }
            }
            return pagesRemoved.ToString();
        }
        public async Task<string> RemoveRepaginateImages(IBrowserFile pdfFile)
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
            StringBuilder pagesRemoved = new StringBuilder();

            var newTitle = pdfFile.Name.Replace(".pdf", "");
            var outputFileToUser = $"{newTitle} Removed on Images.pdf";
            var outputFile = System.IO.Path.Combine("./", outputFileToUser);
            pagesRemoved.Append(outputFile + Environment.NewLine);
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
                                pagesRemoved.Append(page + Environment.NewLine);
                            }
                            else
                            {

                            }
                        }
                        catch (Exception ex)
                        {
                            return $"Unable to parse {filename} - Reason: {ex}";
                        }
                    }
                }
            }
            return pagesRemoved.ToString();
        }
    }
}