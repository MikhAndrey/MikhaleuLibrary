namespace MikhaleuLibrary.Constants
{
    
    /// <summary>
    ///   Describes all constants related to work with files
    /// </summary>
    public static class FileConstants
    {
      
        /// <summary>The user files directory relative path</summary>
        public const string UserFilesDirectoryPath = "../../../";

        /// <summary>The user files directory name</summary>
        public const string UserFilesDirectoryName = "User Files";

        /// <summary>The excel file with books data name</summary>
        public const string ExcelFileName = "LibraryData.xlsx";

        /// <summary>The xml file with books data name</summary>
        public const string XmlFileName = "LibraryData.xml";

        /// <summary>The name of file with the errors of reading from the source file</summary>
        public const string ErrorsFileName = "Errors.txt";

        /// <summary>The symbol that separates adjacent fields in one record in the source file</summary>
        public const char AdjacentFieldsSeparator = ';';

        /// <summary>The symbol that separates line number from error description in the file with errors</summary>
        public const string ErrorContentSeparator = ".     ";

        /// <summary>Required source file extensions string</summary>
        public const string FilesLookupFilter = "CSV Files (*.csv)|*.csv";

        /// <summary>The symbol that separates adjacent errors for one record in the file with errors</summary>
        public const char ErrorMessagesSeparator = ';';
    }

    /// <summary>
    ///   Describes all constants that are used while building UI.
    /// </summary>
    public static class ViewConstants
    {

        /// <summary>The maximum number of errors displaying in the table with errors</summary>
        public const int MaxDisplayingErrorsNumber = 50;
    }

    /// <summary>
    ///   Describes all constants that are used while establishing database connection.
    /// </summary>
    public static class ConnectionConstants
    {
        /// <summary>The connection string name</summary>
        public const string ConnectionStringAlias = "DefaultConnectionString";
    }

    /// <summary>Describes all constants that help to work with main app entities</summary>
    public static class EntityConstants
    {
        /// <summary>The count of book properties</summary>
        public const int BookPropertiesCount = 6;
    }
}

