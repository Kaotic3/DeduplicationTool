using iText.Kernel.Pdf;
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
    public class PDFAnalysis
    {
        private SHA256 Sha256 = SHA256.Create();
        private int MAXALLOWEDSIZE = 15000000;
        public PDFAnalysis()
        {

        }
        public async Task<string> PDFPageCount(MemoryStream pdfFile)
        {
            int pageCount;
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(pdfFile));
            pageCount = pdfDoc.GetNumberOfPages();

            return pageCount.ToString();
        }
        public static string PDFUniquePagesHashImages(string filename)
        {
            return string.Empty;
        }
        public async Task<string> PDFUniquePagesHash(IBrowserFile pdfFile)
        {
            Stream stream = pdfFile.OpenReadStream(MAXALLOWEDSIZE);
            var filename = new MemoryStream();
            await stream.CopyToAsync(filename);
            filename.Position = 0;
            PdfReader pdfReader = new PdfReader(filename);
            PdfDocument pdfDoc = new PdfDocument(pdfReader);

            StringBuilder sb = new StringBuilder();
            for (int page = 1; page <= pdfDoc.GetNumberOfPages(); page++)
            {
                try
                {
                    var strategy = new SimpleTextExtractionStrategy();
                    string pageContent = PdfTextExtractor.GetTextFromPage(pdfDoc.GetPage(page), strategy);
                    var hashValue = pageContent.GetHashCode();

                    sb.Append(hashValue.ToString() + Environment.NewLine);

                }
                catch (Exception ex)
                {
                    return $"Unable to parse {filename} - Reason: {ex}";
                }
            }
            return sb.ToString();
        }
        public async Task<string> PDFUniquePagesShaImages(IBrowserFile pdfFile)
        {
            SHA256 sha256 = SHA256.Create();
            Stream stream = pdfFile.OpenReadStream(MAXALLOWEDSIZE);
            var filename = new MemoryStream();
            await stream.CopyToAsync(filename);
            filename.Position = 0;
            PdfReader pdfReader = new PdfReader(filename);

            PdfDocument pdfDoc = new PdfDocument(pdfReader);
            StringBuilder textBuilder = new StringBuilder();
            StringBuilder imageBuilder = new StringBuilder();

            StringBuilder sb = new StringBuilder();
            for (int page = 1; page <= pdfDoc.GetNumberOfPages(); page++)
            {
                try
                {
                    var strategy = new SimpleTextExtractionStrategy();
                    string pageContent = PdfTextExtractor.GetTextFromPage(pdfDoc.GetPage(page), strategy);
                    pageContent = pageContent.Trim();
                    var hashValue = sha256.ComputeHash(Encoding.UTF8.GetBytes(pageContent));
                    textBuilder.Clear();
                    imageBuilder.Clear();
                    for (int i = 0; i < hashValue.Length; i++)
                    {
                        textBuilder.Append(hashValue[i].ToString("x2"));
                    }
                    var pageToCheck = pdfDoc.GetPage(page);
                    var pageBytes = pageToCheck.GetContentBytes();

                    var imageHash = sha256.ComputeHash(pageBytes);
                    for (int k = 0; k < imageHash.Length; k++)
                    {
                        imageBuilder.Append(imageHash[k].ToString("x2"));
                    }
                    sb.Append(textBuilder.ToString() + imageBuilder.ToString() + Environment.NewLine);
                }
                catch (Exception ex)
                {
                    return $"Unable to parse {filename} - Reason: {ex}";
                }
            }
            return sb.ToString();
        }
        public async Task<string> PDFUniquePagesSha(IBrowserFile pdfFile)
        {
            SHA256 sha256 = SHA256.Create();
            Stream stream = pdfFile.OpenReadStream();
            var filename = new MemoryStream();
            await stream.CopyToAsync(filename);
            
            filename.Position = 0;
            PdfReader pdfReader = new PdfReader(filename);
            PdfDocument pdfDoc = new PdfDocument(pdfReader);

            StringBuilder sb = new StringBuilder();
            for (int page = 1; page <= pdfDoc.GetNumberOfPages(); page++)
            {
                try
                {
                    var strategy = new SimpleTextExtractionStrategy();
                    string pageContent = PdfTextExtractor.GetTextFromPage(pdfDoc.GetPage(page), strategy);
                    pageContent = pageContent.Trim();

                    var pageToCompare = TrimAllWithInplaceCharArray(pageContent);

                    var hashValue = sha256.ComputeHash(Encoding.UTF8.GetBytes(pageToCompare));
                    StringBuilder builder = new StringBuilder();
                    for (int i = 0; i < hashValue.Length; i++)
                    {
                        builder.Append(hashValue[i].ToString("x2"));
                    }

                    sb.Append(builder.ToString() + Environment.NewLine);

                }
                catch (Exception ex)
                {
                    return $"Unable to parse {filename} - Reason: {ex}";
                }
            }
            return sb.ToString();
        }
        public static string RecreatePDF(string filename)
        {
            var path = System.IO.Path.GetDirectoryName(filename);
            var title = System.IO.Path.GetFileName(filename);
            HashSet<string> pages = new HashSet<string>();
            List<int> pagesToCopy = new List<int>();

            PdfReader pdfReader = new PdfReader(filename);
            PdfDocument pdfDoc = new PdfDocument(pdfReader);
            List<int> duplicatePages = new List<int>();
            var newTitle = title.Replace(".pdf", "");
            var outputFile = System.IO.Path.Combine(path, $"{newTitle} Deduplicated.pdf");

            var pageContentToo = "";
            for (int page = 1; page <= pdfDoc.GetNumberOfPages(); page++)
            {
                try
                {

                    var strategy = new SimpleTextExtractionStrategy();
                    string pageContent = PdfTextExtractor.GetTextFromPage(pdfDoc.GetPage(page), strategy);

                    var hashValue = pageContent.GetHashCode();
                    if (!pages.Contains(hashValue.ToString()))
                    {
                        pagesToCopy.Add(page);
                        pages.Add(hashValue.ToString());
                    }
                    else
                    {
                        duplicatePages.Add(page);
                    }
                }
                catch (Exception ex)
                {
                    return $"Unable to parse {filename} - Reason: {ex}";
                }
            }
            using (var pdfIn = new PdfDocument(new PdfReader(filename)))
            {
                using (var pdfOut = new PdfDocument(new PdfWriter(outputFile)))
                {
                    foreach (var pageToCopy in pagesToCopy)
                    {
                        pdfIn.CopyPagesTo(pageToCopy, pageToCopy, pdfOut, pageToCopy + 1);
                    }

                    Document document = new Document(pdfOut);
                    for (int i = 1; i <= pages.Count(); i++)
                    {
                        document.ShowTextAligned(new Paragraph(String.Format("Page " + i + " of " + pages.Count())), 559, 806, i, TextAlignment.RIGHT, VerticalAlignment.TOP, 0);
                    }
                }
            }
            StringBuilder sbCopyPagse = new StringBuilder();

            foreach (var item in duplicatePages)
            {
                sbCopyPagse.Append($"Page: {item}" + Environment.NewLine);
            }

            return $"Duplicated pages removed" + Environment.NewLine + $"{sbCopyPagse.ToString()}";
        }
        public static string TrimAllWithInplaceCharArray(string str)
        {
            var len = str.Length;
            var src = str.ToCharArray();
            int dstIdx = 0;

            for (int i = 0; i < len; i++)
            {
                var ch = src[i];

                switch (ch)
                {

                    case '\u0020':
                    case '\u00A0':
                    case '\u1680':
                    case '\u2000':
                    case '\u2001':

                    case '\u2002':
                    case '\u2003':
                    case '\u2004':
                    case '\u2005':
                    case '\u2006':

                    case '\u2007':
                    case '\u2008':
                    case '\u2009':
                    case '\u200A':
                    case '\u202F':

                    case '\u205F':
                    case '\u3000':
                    case '\u2028':
                    case '\u2029':
                    case '\u0009':

                    case '\u000A':
                    case '\u000B':
                    case '\u000C':
                    case '\u000D':
                    case '\u0085':
                        continue;

                    default:
                        src[dstIdx++] = ch;
                        break;
                }
            }
            return new string(src, 0, dstIdx);
        }
    }
}