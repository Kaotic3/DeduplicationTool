using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeduplicationTool.Models
{
    public class Pagination
    {
        public Pagination()
        {

        }
        public static string PaginateDocument(string filename)
        {
            var path = System.IO.Path.GetDirectoryName(filename);
            var title = System.IO.Path.GetFileName(filename);
            var cleanedTitle = title.Replace(".pdf", "");
            var outputFileToUser = $"{cleanedTitle} Paginated.pdf";
            var outputFile = System.IO.Path.Combine(path, outputFileToUser);
            try
            {
                using (var pdfOut = new PdfDocument(new PdfReader(filename), new PdfWriter(outputFile)))
                {
                    Document document = new Document(pdfOut);

                    for (int page = 1; page <= pdfOut.GetNumberOfPages(); page++)
                    {
                        Rectangle rectangle = pdfOut.GetPage(page).GetPageSize();
                        var width = rectangle.GetWidth();
                        var middle = width / 2;

                        document.ShowTextAligned(new Paragraph(String.Format("Page " + page + " of " + pdfOut.GetNumberOfPages())), middle, 7, page, TextAlignment.CENTER, VerticalAlignment.BOTTOM, 0);
                    }
                }
            }
            catch (Exception ex)
            {
                return $"Failed to repaginate document due to {ex.Message}";
            }
            return outputFile;
        }
        public static string RepaginateDocument(string filename, int pageCount, List<int> pagesRemoved)
        {
            var path = System.IO.Path.GetDirectoryName(filename);
            var title = System.IO.Path.GetFileName(filename);
            var cleanedTitle = title.Replace(".pdf", "");
            var outputFileToUser = $"{cleanedTitle} Repaginated.pdf";
            var outputFile = System.IO.Path.Combine(path, outputFileToUser);
            List<string> pageToCount = new List<string>();
            int k = 1;
            try
            {
                using (var pdfOut = new PdfDocument(new PdfReader(filename), new PdfWriter(outputFile)))
                {
                    Document document = new Document(pdfOut);

                    for (int page = 1; page <= pdfOut.GetNumberOfPages(); page++)
                    {
                        if (pagesRemoved.Contains(k))
                        {
                            k++;
                        }
                        Rectangle rectangle = pdfOut.GetPage(page).GetPageSize();
                        var width = rectangle.GetWidth();
                        var middle = width / 2;
                        document.ShowTextAligned(new Paragraph(String.Format("Page " + k + " of " + pageCount)), middle, 7, page, TextAlignment.CENTER, VerticalAlignment.BOTTOM, 0);
                        k++;
                    }
                }
            }
            catch (Exception ex)
            {
                return $"Failed to repaginate document due to {ex.Message}";
            }

            return outputFile;
        }

        // TODO: You got it for a single page - now do it for multiple pages.


        public static string RepaginateMovedPages(string filename, int pageCount, List<int> pagesMoved)
        {
            var path = System.IO.Path.GetDirectoryName(filename);
            var title = System.IO.Path.GetFileName(filename);
            var cleanedTitle = title.Replace(".pdf", "");
            var outputFileToUser = $"{cleanedTitle} Repaginated.pdf";
            var outputFile = System.IO.Path.Combine(path, outputFileToUser);
            List<int> pageToCount = new List<int>();
            List<int> pagesToMiss = new List<int>();
            int k = 1;
            try
            {
                using (var pdfOut = new PdfDocument(new PdfReader(filename), new PdfWriter(outputFile)))
                {
                    Document document = new Document(pdfOut);

                    foreach (var pageInOrder in pagesMoved)
                    {
                        if (pageInOrder == 99999999)
                        {
                            k++;
                        }
                        else
                        {
                            Rectangle rectangle = pdfOut.GetPage(k).GetPageSize();
                            var width = rectangle.GetWidth();
                            var middle = width / 2;
                            document.ShowTextAligned(new Paragraph(String.Format("Page New " + pageInOrder + " of " + pageCount)), middle, 7, k, TextAlignment.CENTER, VerticalAlignment.BOTTOM, 0);
                            k++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return $"Failed to paginate {filename} due to {ex.Message}";
            }

            return outputFile;
        }
    }
}