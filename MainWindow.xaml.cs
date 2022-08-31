using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Microsoft.Win32;
using MikhaleuLibrary.Constants;
using MikhaleuLibrary.Services;
using MikhaleuLibrary.Model.DBModels;
using MikhaleuLibrary.Repositories.Interfaces;
using MikhaleuLibrary.Repositories;
using MikhaleuLibrary.Resources;
using MikhaleuLibrary.Model.ViewModel;
using System.Collections.ObjectModel;
using MikhaleuLibrary.Helpers;
using MikhaleuLibrary.Utils;

namespace MikhaleuLibrary
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            errorsGrid.ItemsSource = _errors;
            errorsGrid.IsReadOnly = true;
            errorsView.Visibility = Visibility.Collapsed;
            filePathStringContainer.Visibility = Visibility.Collapsed;
            loadToDBButton.Visibility = Visibility.Collapsed;
            errorsTableCaption.Text = String.Format(Messages.ErrorsTableCaption, ViewConstants.MaxDisplayingErrorsNumber);
            requestsSectionCaption.Text = Messages.RequestsSectionCaption;
            firstNameLabel.Text = Messages.FirstNameLabel;
            lastNameLabel.Text = Messages.LastNameLabel;    
            surnameLabel.Text = Messages.SurnameLabel;  
            birthDateLabel.Text = Messages.BirthDateLabel;
            bookNameLabel.Text = Messages.BookNameLabel;
            bookYearLabel.Text = Messages.BookYearLabel;
            fileSelectButton.Content = Messages.FileSelectButtonCaption;
            loadToDBButton.Content = Messages.DBLoadButtonCaption;
            lineNumberColumn.Header = Messages.LineNumberColumnCaption; 
            errorDescriptionColumn.Header = Messages.ErrorDescriptionColumnCaption;
            loadToExcelButton.Content = Messages.ExcelButtonCaption;
            loadToXMLButton.Content = Messages.XMLButtonCaption; 
            loadToExcelButton.Visibility = Visibility.Collapsed;
            loadToXMLButton.Visibility = Visibility.Collapsed;
            getFilteredDataButton.Content = Messages.FilterButtonCaption;
        }

        private string? _sourceFileName = null;

        public ObservableCollection<ReadFromFileError> _errors = new();

        private const string _errorsFilePath = _userFilesDirectoryPath + _pathSeparator + FileConstants.ErrorsFileName;

        private const string _userFilesDirectoryPath = FileConstants.UserFilesDirectoryPath + FileConstants.UserFilesDirectoryName;

        private List<string> _alreadyReadFileNames = new();

        private List<Book> _filteredBooks = new();

        private List<Book> _booksFromFile = new();

        private const string _pathSeparator = "/";

        private void UpdateErrorsViewVisibility()
        {
            if (_errors.Count == 0)
            {
                errorsView.Visibility = Visibility.Collapsed;
            }
            else
            {
                errorsView.Visibility = Visibility.Visible;
            }
        }

        private void File_Select_Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = FileConstants.FilesLookupFilter;
            if (openFileDialog.ShowDialog() == true)
            {
                _sourceFileName = openFileDialog.FileName;
                filePathString.Text = Messages.FileNameDeclaration + openFileDialog.FileName;
                filePathStringContainer.Visibility = Visibility.Visible;
                loadToDBButton.Visibility = Visibility.Visible;
                requestsView.Visibility = Visibility.Visible;
            }
        }

        private bool IsPreparationForDataLoadSuccessful() {
            if (_alreadyReadFileNames.Contains(_sourceFileName))
            {
                MessageBoxResult loadFileConfirm = MessageBox.Show(Messages.FileWasReadBefore,
                    Messages.ReadFromFileErrorWarning,
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);
                if (loadFileConfirm == MessageBoxResult.No)
                    return false;
            }
            try
            {
                if (!Directory.Exists(_userFilesDirectoryPath))
                    Directory.CreateDirectory(_userFilesDirectoryPath);
            }
            catch
            {
                MessageBox.Show(Messages.UnableToCreateDirectoryMessage,
                    Messages.DirectoryCreationFaultCaption,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return false;
            }
            FileToDBSupplier.EndOfFileReached = false;
            _errors.Clear();
            _booksFromFile.Clear();
            return true;
        }

        private async Task LoadBooksInfoFromFile()
        {
            int errorsCount = 0;
            using (StreamReader reader = new(_sourceFileName))
            {
                using (StreamWriter writer = new(_errorsFilePath, false))
                {
                    int currentRowIndex = 1;
                    try
                    {
                        await writer.WriteLineAsync(DateTime.Now.ToString());
                        for (int i = 0; i < 1000000; i++)
                            await writer.WriteLineAsync("Евгения;Ковалева;Максимовна;03.03.1983;Анус Быка;2005");
                    }
                    catch
                    {
                        MessageBox.Show(Messages.WriteToErrorsFileFault,
                            Messages.WriteToFileErrorDescription,
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                        return;
                    }
                    Book? book;
                    while (true)
                    {
                        try
                        {
                            book = await FileToDBSupplier.GetBookFromFile(reader);
                        }
                        catch
                        {
                            MessageBox.Show(Messages.ReadFromSourceFileFault,
                                Messages.ReadFromFileErrorDescription,
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                            return;
                        }
                        if (book == null)
                        {
                            if (FileToDBSupplier.EndOfFileReached)
                            {
                                _alreadyReadFileNames.Add(_sourceFileName);
                                return;
                            }
                            try
                            {
                                FileToDBSupplier.WriteErrorsToFile(writer, currentRowIndex, FileToDBSupplier.ConversionErrorDescription);
                                if (errorsCount <= ViewConstants.MaxDisplayingErrorsNumber)
                                {
                                    errorsCount++;
                                    _errors.Add(new ReadFromFileError(currentRowIndex, FileToDBSupplier.ConversionErrorDescription));
                                }
                            }
                            catch
                            {
                                MessageBox.Show(Messages.WriteToErrorsFileFault,
                                    Messages.WriteToFileErrorDescription,
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                                return;
                            }
                        }
                        else
                        {
                            _booksFromFile.Add(book);
                        }
                        currentRowIndex++;
                    }
                }
            }
        }

        private async Task LoadBooksInfoToDB()
        {
            try
            {
                using (IRepository<Book> _booksDB = new BooksRepository())
                {
                    await Task.Run(() =>
                    {
                        int booksCount = _booksFromFile.Count;
                        for (int i=0; i < booksCount; i++)
                            _booksDB.Add(_booksFromFile[i]);
                        _booksDB.Save();
                    });
                    MessageBox.Show(Messages.SuccessfulDBWriteMessage + FileConstants.UserFilesDirectoryName + 
                        _pathSeparator + FileConstants.ErrorsFileName,
                        Messages.SuccessMessage,
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
            }
            catch
            {
                MessageBox.Show(Messages.DatabaseSaveError,
                    Messages.DatabaseErrorDescription,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private async void Load_From_DB_Button_Click(object sender, RoutedEventArgs e)
        { 
            bool CanDataBeLoadedFromFile = IsPreparationForDataLoadSuccessful();
            if (CanDataBeLoadedFromFile)
            {
                loadToDBButton.IsEnabled = false;
                await LoadBooksInfoFromFile();
                await LoadBooksInfoToDB();
                loadToDBButton.IsEnabled = true;
                UpdateErrorsViewVisibility();
            }
        }

        private async void Get_Filtered_Data_Button_Click(object sender, RoutedEventArgs e)
        {
            string firstName = firstNameInput.Text.Trim();
            string lastName = lastNameInput.Text.Trim();   
            string surname = surnameInput.Text.Trim(); 
            string birthDate = birthDateInput.Text.Trim(); 
            string bookName = bookNameInput.Text.Trim();
            string bookYear = bookYearInput.Text.Trim();
            bool?[] includedParamsState = {firstNameCheck.IsChecked,
                surnameCheck.IsChecked,
                lastNameCheck.IsChecked,
                birthDateCheck.IsChecked,
                bookNameCheck.IsChecked,
                bookYearCheck.IsChecked};
            if (Array.IndexOf(includedParamsState,true) == -1)
            {
                MessageBox.Show(Messages.NoMarkedFieldsMessage,
                    Messages.RequestErrorDeclaration,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }
            string[] bookProperties = { firstName, surname, lastName, birthDate, bookName, bookYear };
            bool isUserInputCorrect = BookPropertyChecker.IsUserRequestProper(bookProperties,
                includedParamsState,
                out string? errorDescription,
                out DateTime? correctBirthDate,
                out int correctBookYear);
            if (!isUserInputCorrect)
            {
                MessageBox.Show(Messages.WrongRequestMessage + errorDescription,
                    Messages.RequestErrorDeclaration,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }
            
            using (IRepository<Book> _booksDB = new BooksRepository()) {
                IEnumerable<Book> booksFromDB;
                IEnumerable<Book> localFilteredBooks = null;    
                await Task.Run(() =>
                {
                    try
                    {
                        booksFromDB = _booksDB.GetAll();
                    }
                    catch
                    {
                        MessageBox.Show(Messages.DatabaseGetDataError,
                            Messages.DatabaseErrorDescription,
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                        return;
                    }
                    localFilteredBooks = booksFromDB.Where(book => (book.FirstName == firstName || firstNameCheck.IsChecked != true) &&
                    (book.LastName == lastName || lastNameCheck.IsChecked != true) &&
                    (book.Surname == surname || surnameCheck.IsChecked != true) &&
                    (book.BirthDate == correctBirthDate || birthDateCheck.IsChecked != true) &&
                    (book.BookName == bookName || bookNameCheck.IsChecked != true) &&
                    (book.BookYear == correctBookYear || bookYearCheck.IsChecked != true));
                });         
                _filteredBooks = localFilteredBooks.ToList();
                MessageBox.Show(Messages.SuccessfulDataFiltration,
                    Messages.SuccessMessage,
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                if (_filteredBooks.Count == 0)
                {
                    loadToExcelButton.Visibility = Visibility.Collapsed;
                    loadToXMLButton.Visibility = Visibility.Collapsed;
                    MessageBox.Show(Messages.EmptyResponseMessage,
                        Messages.RequestErrorDeclaration,
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
                else
                {
                    loadToExcelButton.Visibility = Visibility.Visible;
                    loadToXMLButton.Visibility = Visibility.Visible;
                }
            }
        }

        private void Load_To_Excel_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!Directory.Exists(_userFilesDirectoryPath))
                    Directory.CreateDirectory(_userFilesDirectoryPath);
            }
            catch
            {
                MessageBox.Show(Messages.UnableToCreateDirectoryMessage,
                    Messages.DirectoryCreationFaultCaption,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }
            const string pathSeparator = "/";
            const string ExcelFilePath = _userFilesDirectoryPath + pathSeparator + FileConstants.ExcelFileName;
            try{
                FileHandler.AddBooksToExcelFile(_filteredBooks, ExcelFilePath);
            }
            catch
            {
                MessageBox.Show(Messages.WriteToExcelFileFault,
                    Messages.WriteToFileErrorDescription,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }
            MessageBox.Show(Messages.SuccessfulExcelFileWriteMessage + FileConstants.UserFilesDirectoryName + pathSeparator + FileConstants.ExcelFileName,
                Messages.SuccessMessage,
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void Load_To_XML_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!Directory.Exists(_userFilesDirectoryPath))
                    Directory.CreateDirectory(_userFilesDirectoryPath);
            }
            catch
            {
                MessageBox.Show(Messages.UnableToCreateDirectoryMessage,
                    Messages.DirectoryCreationFaultCaption,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }
            const string pathSeparator = "/";
            const string xmlFilePath = _userFilesDirectoryPath + pathSeparator + FileConstants.XmlFileName;
            try
            {
                FileHandler.AddBooksToXMLFile(_filteredBooks, xmlFilePath);
            }
            catch
            {
                MessageBox.Show(Messages.WriteToXMLFileFault,
                    Messages.WriteToFileErrorDescription,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }
            MessageBox.Show(Messages.SuccessfulXMLFileWriteMessage + FileConstants.UserFilesDirectoryName + pathSeparator + FileConstants.XmlFileName,
                Messages.SuccessMessage,
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
    }
}
