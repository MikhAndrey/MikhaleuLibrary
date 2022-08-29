using MikhaleuLibrary.Resources;
using MikhaleuLibrary.Constants;
using System;

namespace MikhaleuLibrary.Helpers
{
    public static class BookPropertyChecker
    {
        private static char _errorMessagesSeparator = FileConstants.ErrorMessagesSeparator;

        public static bool IsBookDataProper(string[] bookProperties, out string? errorDescription, out DateTime? authorBirthdate, out int bookYear)
        {
            errorDescription = null;
            authorBirthdate = null;
            bookYear = 0;
            bool[] bookPropertyChecks = {!IsPropertiesCountOptimal(bookProperties, ref errorDescription),
                !IsAuthorNameCorrect(bookProperties[0], ref errorDescription),
                !IsAuthorSurnameCorrect(bookProperties[1], ref errorDescription),
                !IsAuthorBirthdateCorrect(bookProperties[3], ref errorDescription, ref authorBirthdate),
                !IsBookNameCorrect(bookProperties[4], ref errorDescription),
                !IsBookYearCorrect(bookProperties[5], ref errorDescription, ref bookYear) };
            if (Array.IndexOf(bookPropertyChecks, true) != -1)
                return false;
            return true;
        }

        private static bool IsPropertiesCountOptimal(string[] properties, ref string? errorDescription)
        {
            if (properties.Length == EntityConstants.BookPropertiesCount)
                return true;
            else
            {
                errorDescription += Messages.LackOfParamsMessage + _errorMessagesSeparator;
                return false;
            }
        }

        private static bool IsAuthorNameCorrect(string? authorName, ref string? errorDescription)
        {
            if (!string.IsNullOrWhiteSpace(authorName))
                return true;
            else
            {
                errorDescription += Messages.EmptyAuthorNameMessage + _errorMessagesSeparator;
                return false;
            }
        }

        private static bool IsAuthorSurnameCorrect(string? authorSurname, ref string? errorDescription)
        {
            if (!string.IsNullOrWhiteSpace(authorSurname))
                return true;
            else
            {
                errorDescription += Messages.EmptyAuthorSurnameMessage + _errorMessagesSeparator;
                return false;
            }
        }

        private static bool IsAuthorBirthdateCorrect(string? authorBirthdate, ref string? errorDescription, ref DateTime? correctAuthorBirthdate)
        {
            if (string.IsNullOrWhiteSpace(authorBirthdate))
            {
                errorDescription += Messages.EmptyBirthdateMessage + _errorMessagesSeparator;
                return false;
            }
            try
            {
                correctAuthorBirthdate = DateTime.Parse(authorBirthdate);
            }
            catch
            {
                errorDescription += Messages.WrongBirthdateConversionMessage + _errorMessagesSeparator;
                return false;
            }
            DateTime actualDate = DateTime.Now;
            if (correctAuthorBirthdate >= actualDate)
            {
                errorDescription += Messages.FutureAuthorBirthdateMessage + _errorMessagesSeparator;
                return false;
            }
            return true;
        }

        private static bool IsBookNameCorrect(string? bookName, ref string? errorDescription)
        {
            if (!string.IsNullOrWhiteSpace(bookName))
                return true;
            else
            {
                errorDescription += Messages.EmptyBookNameMessage + _errorMessagesSeparator;
                return false;
            }
        }

        private static bool IsBookYearCorrect(string? bookYear, ref string? errorDescription,  ref int realBookYear)
        {
            if (string.IsNullOrWhiteSpace(bookYear))
            {
                errorDescription += Messages.EmptyBookYearMessage + _errorMessagesSeparator;
                return false;
            }
            bool isYearInteger = int.TryParse(bookYear, out realBookYear);
            if (!isYearInteger)
            {
                errorDescription += Messages.NonIntegerYearMessage + _errorMessagesSeparator;
                return false;
            }
            DateTime actualDate = DateTime.Now;
            if (realBookYear > actualDate.Year)
            {
                errorDescription += Messages.FutureBookYearMessage + _errorMessagesSeparator;
                return false;
            }
            return true;
        }
    }
}
