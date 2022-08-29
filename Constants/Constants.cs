namespace MikhaleuLibrary.Constants
{
    
    /// <summary>
    ///   This class contains all required game constants
    /// </summary>
    public static class FileConstants
    {
      
        /// <summary>The user files directory path</summary>
        public const string UserFilesDirectoryPath = "../../../";

        public const string UserFilesDirectoryName = "User Files";

        public const string ExcelFileName = "LibraryData.xls";

        public const string XmlFileName = "LibraryData.xml";

        public const string ConnectionStringAlias = "DefaultConnectionString";

        public const string ErrorsFileName = "Errors.txt";

        public const char AdjacentFieldsSeparator = ';';

        public const string ErrorContentSeparator = ".     ";

        public const string FilesLookupFilter = "CSV Files (*.csv)|*.csv";

        public const char ErrorMessagesSeparator = ';';
    }

    public static class ViewConstants
    {

        public const int MaxDisplayingErrorsNumber = 50;

    }

    public static class ConnectionConstants
    {
        public const string ConnectionStringAlias = "DefaultConnectionString";
    }

    public static class EntityConstants
    {
        public const int BookPropertiesCount = 6;

    }
}

