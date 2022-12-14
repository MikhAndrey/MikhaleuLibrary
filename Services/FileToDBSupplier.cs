using System;
using System.IO;
using System.Threading.Tasks;
using MikhaleuLibrary.Helpers;
using MikhaleuLibrary.Constants;
using MikhaleuLibrary.Model.DBModels;
using MikhaleuLibrary.Utils;

namespace MikhaleuLibrary.Services
{
    public static class FileToDBSupplier
    {
        public static string? ConversionErrorDescription = null;

        public static bool EndOfFileReached;

        private static char _adjacentFieldsSeparator = FileConstants.AdjacentFieldsSeparator;

        private static char _errorMessagesSeparator = FileConstants.ErrorMessagesSeparator;

        public static async Task<Book?> GetBookFromFile(StreamReader reader)
        {
            string? posiibleBookItem = await FileHandler.ReadStringFromTextFile(reader);
            if (posiibleBookItem == null)
            {
                EndOfFileReached = true;
                return null;
            }
            string[] possibleBookProperties = posiibleBookItem.Split(_adjacentFieldsSeparator);
            foreach (string property in possibleBookProperties)
                property.Trim();
            if (!BookPropertyChecker.IsBookDataFromFileProper(possibleBookProperties, out ConversionErrorDescription, out DateTime? birthDate, out int bookYear))
            {
                ConversionErrorDescription = ConversionErrorDescription.TrimEnd(_errorMessagesSeparator);
                return null;
            }
            else
                return new Book(possibleBookProperties[0],
                    possibleBookProperties[1],
                    possibleBookProperties[2],
                    (DateTime)birthDate,
                    possibleBookProperties[4],
                    bookYear);
        }

        public static async void WriteErrorsToFile(StreamWriter errorsWriter, int rowIndex, string errorMessage) {
            await Task.Run(()=>FileHandler.WriteStringToTextFile(errorsWriter, rowIndex.ToString() + FileConstants.ErrorContentSeparator + errorMessage));
        }
    }
}
