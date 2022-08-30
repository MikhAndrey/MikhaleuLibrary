using System.IO;
using System.Threading.Tasks;
using MikhaleuLibrary.Model.DBModels;
using System.Xml.Linq;
using Excel = Microsoft.Office.Interop.Excel;
using System.Collections.Generic;
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

        public static void AddBookToExcelFile(Excel.Worksheet sheet, int rowIndex, Book book)
        {
            sheet.Cells[rowIndex, 0] = book.FirstName;
            sheet.Cells[rowIndex, 1] = book.Surname;
            sheet.Cells[rowIndex, 2] = book.LastName;
            sheet.Cells[rowIndex, 3] = book.BirthDate;
            sheet.Cells[rowIndex, 2] = book.BookName;
            sheet.Cells[rowIndex, 2] = book.BookYear;
        }
    }
}
