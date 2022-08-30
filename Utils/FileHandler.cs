using System.IO;
using System.Threading.Tasks;
using MikhaleuLibrary.Model.DBModels;
using System.Xml.Linq;
using System.Collections.Generic;
using System;
using OfficeOpenXml;

namespace MikhaleuLibrary.Utils
{

    /// <summary>
    /// Performs basic actions with <see cref="Console"/> object. 
    /// </summary>
    public static class FileHandler
    {
        public static async Task<string?> ReadStringFromTextFile(StreamReader reader)
        {
            string? currentString = await reader.ReadLineAsync();
            return currentString;
        }

        public static async void WriteStringToTextFile(StreamWriter writer, string text)
        {
            await writer.WriteLineAsync(text);
        }

        public static void AddBooksToXMLFile(List<Book> books, string filePath)
        {
            XDocument xdoc = new XDocument();
            XElement libraryElem = new("Library");
            xdoc.Add(libraryElem);
            foreach (Book book in books)
            {

                XElement bookElem = new XElement("Book",
                    new XAttribute("id", book.Id),
                    new XElement("FirstName", book.FirstName),
                    new XElement("Surname", book.Surname),
                    new XElement("LastName", book.LastName),
                    new XElement("Birthdate", book.BirthDate),
                    new XElement("BookName", book.BookName),
                    new XElement("BookYear", book.BookYear));
                libraryElem.Add(bookElem);
            }
            xdoc.Save(filePath);
        }

        public static void AddBooksToExcelFile(List<Book> books, string fileName)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
                ExcelWorksheet sheet = package.Workbook.Worksheets.Add("Books Info");
                sheet.Cells["A1"].Value = "First name";
                sheet.Cells["B1"].Value = "Surname";
                sheet.Cells["C1"].Value = "Last name";
                sheet.Cells["D1"].Value = "Author birthdate";
                sheet.Cells["E1"].Value = "Book name";
                sheet.Cells["F1"].Value = "Book year";
                int booksCount = books.Count;
                for (int i = 0; i < booksCount; i++)
                {
                    sheet.Cells[i + 1, 1].Value = books[i].FirstName;
                    sheet.Cells[i + 1, 2].Value = books[i].Surname;
                    sheet.Cells[i + 1, 3].Value = books[i].LastName;
                    sheet.Cells[i + 1, 4].Value = books[i].BirthDate.ToString();
                    sheet.Cells[i + 1, 5].Value = books[i].BookName;
                    sheet.Cells[i + 1, 6].Value = books[i].BookYear;
                }
                package.SaveAs(new FileInfo(fileName));
            }
        }
    }
}
