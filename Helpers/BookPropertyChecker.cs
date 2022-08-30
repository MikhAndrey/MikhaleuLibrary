using MikhaleuLibrary.Resources;
using MikhaleuLibrary.Constants;
using System;

namespace MikhaleuLibrary.Helpers
{
    public static class BookPropertyChecker
    {
        private static char _errorMessagesSeparator = FileConstants.ErrorMessagesSeparator;

        public static bool IsBookDataFromFileProper(string[] bookProperties, out string? errorDescription, out DateTime? authorBirthdate, out int bookYear)
        {
            errorDescription = null;
            authorBirthdate = null;
            bookYear = 0;
            if (!IsPropertiesCountOptimal(bookProperties))
                errorDescription += Messages.LackOfParamsMessage + _errorMessagesSeparator;
            if (!IsAuthorNameNotNullOrWhiteSpace(bookProperties[0]))
                errorDescription += Messages.EmptyAuthorNameMessage + _errorMessagesSeparator;
            if (!IsAuthorSurnameNotNullOrWhiteSpace(bookProperties[1]))
                errorDescription += Messages.EmptyAuthorSurnameMessage + _errorMessagesSeparator;
            if (!IsDateNotNullOrWhiteSpace(bookProperties[3]))
                errorDescription += Messages.EmptyBirthdateMessage + _errorMessagesSeparator;
            if (!IsDateCorrect(bookProperties[3], ref authorBirthdate))
                errorDescription += Messages.WrongBirthdateConversionMessage + _errorMessagesSeparator;
            if (!IsDateFromPast(ref authorBirthdate))
                errorDescription += Messages.FutureAuthorBirthdateMessage + _errorMessagesSeparator;
            if (!IsBookNameNotNullOrWhiteSpace(bookProperties[4]))
                errorDescription += Messages.EmptyBookNameMessage + _errorMessagesSeparator;
            if (!IsYearNotNullOrWhiteSpace(bookProperties[5]))
                errorDescription += Messages.EmptyBookYearMessage + _errorMessagesSeparator;
            if (!IsYearInteger(bookProperties[5], ref bookYear))
                errorDescription += Messages.NonIntegerYearMessage + _errorMessagesSeparator;
            if (!IsYearFromPast(bookYear))
                errorDescription += Messages.NonIntegerYearMessage + _errorMessagesSeparator;
            if (errorDescription != null)
                return false;
            return true;
        }

        public static bool IsUserRequestProper(string[] bookProperties, bool?[] isParamNeedsCheck, out string? errorDescription, out DateTime? authorBirthdate, out int bookYear)
        {
            errorDescription = null;
            authorBirthdate = null;
            bookYear = 0;
            if (isParamNeedsCheck[0] == true)
            {
                if (!IsAuthorNameNotNullOrWhiteSpace(bookProperties[0]))
                    errorDescription += Messages.EmptyAuthorNameMessage + _errorMessagesSeparator;
            }
            if (isParamNeedsCheck[1] == true)
            {
                if (!IsAuthorSurnameNotNullOrWhiteSpace(bookProperties[1]))
                    errorDescription += Messages.EmptyAuthorSurnameMessage + _errorMessagesSeparator;
            }
            if (isParamNeedsCheck[3] == true)
            {
                if (!IsDateNotNullOrWhiteSpace(bookProperties[3]))
                    errorDescription += Messages.EmptyBirthdateMessage + _errorMessagesSeparator;
                if (!IsDateCorrect(bookProperties[3], ref authorBirthdate))
                    errorDescription += Messages.WrongBirthdateConversionMessage + _errorMessagesSeparator;
                if (!IsDateFromPast(ref authorBirthdate))
                    errorDescription += Messages.FutureAuthorBirthdateMessage + _errorMessagesSeparator;
            }
            if (isParamNeedsCheck[4] == true)
            {
                if (!IsBookNameNotNullOrWhiteSpace(bookProperties[4]))
                    errorDescription += Messages.EmptyBookNameMessage + _errorMessagesSeparator;
            }
            if (isParamNeedsCheck[5] == true)
            {
                if (!IsYearNotNullOrWhiteSpace(bookProperties[5]))
                    errorDescription += Messages.EmptyBookYearMessage + _errorMessagesSeparator;
                if (!IsYearInteger(bookProperties[5], ref bookYear))
                    errorDescription += Messages.NonIntegerYearMessage + _errorMessagesSeparator;
                if (!IsYearFromPast(bookYear))
                    errorDescription += Messages.FutureBookYearMessage + _errorMessagesSeparator;
            }
            if (errorDescription != null)
                return false;
            return true;
        }

        private static bool IsPropertiesCountOptimal(string[] properties)
        {
            if (properties.Length == EntityConstants.BookPropertiesCount)
                return true;
            return false;
        }

        private static bool IsAuthorNameNotNullOrWhiteSpace(string? authorName)
        {
            if (!string.IsNullOrWhiteSpace(authorName))
                return true;
            return false;
        }

        private static bool IsAuthorSurnameNotNullOrWhiteSpace(string? authorSurname)
        {
            if (!string.IsNullOrWhiteSpace(authorSurname))
                return true;
            return false;
        }

        private static bool IsDateNotNullOrWhiteSpace(string? date)
        {
            if (!string.IsNullOrWhiteSpace(date))
                return true;
            return false;
        }

        private static bool IsDateCorrect(string? date, ref DateTime? correctDate)
        {
            try
            {
                correctDate = DateTime.Parse(date);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static bool IsDateFromPast(ref DateTime? date)
        {
            DateTime actualDate = DateTime.Now;
            if (date >= actualDate)
                return false;
            return true;
        }

        private static bool IsBookNameNotNullOrWhiteSpace(string? bookName)
        {
            if (!string.IsNullOrWhiteSpace(bookName))
                return true;
            return false;
        }

        private static bool IsYearNotNullOrWhiteSpace(string? year)
        {
            if (!string.IsNullOrWhiteSpace(year))
                return true;
            return false;
        }

        private static bool IsYearInteger(string? year, ref int correctYear)
        {
            bool isYearInteger = int.TryParse(year, out correctYear);
            if (!isYearInteger)
                return false;
            return true;
        }

        private static bool IsYearFromPast(int year)
        {
            DateTime actualDate = DateTime.Now;
            if (year > actualDate.Year)
                return false;
            return true;
        }
    }
}
