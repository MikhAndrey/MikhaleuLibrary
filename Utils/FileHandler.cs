using System.IO;
using System.Threading.Tasks;
using MikhaleuLibrary.Model.DBModels;
using System.Xml.Serialization;
using Excel = Microsoft.Office.Interop.Excel;
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

        public static void AddBookToXMLFile(FileStream file, XmlSerializer writer, Book book)
        {
            writer.Serialize(file, book);
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
