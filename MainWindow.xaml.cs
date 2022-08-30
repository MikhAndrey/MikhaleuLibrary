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

        private List<Book> _filteredBooks;

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

        private async void Load_From_DB_Button_Click(object sender, RoutedEventArgs e)
        {
            loadToDBButton.IsEnabled = false;
            _errors.Clear();
            const string userFilesDirectoryPath = FileConstants.UserFilesDirectoryPath + FileConstants.UserFilesDirectoryName;
            try
            {
                if (!Directory.Exists(userFilesDirectoryPath))
                    Directory.CreateDirectory(userFilesDirectoryPath);
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
            const string errorsFilePath = userFilesDirectoryPath + pathSeparator + FileConstants.ErrorsFileName;
            FileToDBSupplier supplier = new();
            supplier.EndOfFileReached = false;
            using (StreamReader reader = new(_sourceFileName))
            {
                using (StreamWriter writer = new(errorsFilePath, false))
                {
                    int currentRowIndex = 1;
                    try
                    {
                        await writer.WriteLineAsync(DateTime.Now.ToString());
                    }
                    catch
                    {
                        MessageBox.Show(Messages.WriteToErrorsFileFault,
                            Messages.WriteToFileErrorDescription,
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                        loadToDBButton.IsEnabled = true;
                        return;
                    }
                    Book? book;
                    while (true)
                    {
                        try
                        {
                            book = await supplier.GetBookFromFile(reader);
                        }
                        catch
                        {
                            MessageBox.Show(Messages.ReadFromSourceFileFault,
                                Messages.ReadFromFileErrorDescription,
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                            loadToDBButton.IsEnabled = true;
                            return;
                        }
                        if (supplier.EndOfFileReached)
                        {
                            MessageBox.Show(Messages.SuccessfulDBWriteMessage + FileConstants.UserFilesDirectoryName + pathSeparator + FileConstants.ErrorsFileName,
                                Messages.SuccessMessage,
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
                            loadToDBButton.IsEnabled = true;
                            UpdateErrorsViewVisibility();
                            return;
                        }
                        if (book != null)
                        {
                            try
                            {
                                using (IRepository<Book> _booksDB = new BooksRepository())
                                {
                                    _booksDB.Add(book);
                                    _booksDB.Save();
                                }
                            }
                            catch
                            {
                                MessageBox.Show(Messages.DatabaseSaveError,
                                    Messages.DatabaseErrorDescription,
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                                loadToDBButton.IsEnabled = true;
                                return;
                            }
                        }
                        else
                        {
                            try
                            {
                                supplier.WriteErrorsToFile(writer, currentRowIndex, supplier.ConversionErrorDescription);
                            }
                            catch
                            {
                                MessageBox.Show(Messages.WriteToErrorsFileFault,
                                    Messages.WriteToFileErrorDescription,
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                                loadToDBButton.IsEnabled = true;
                                return;
                            }
                            if (currentRowIndex < ViewConstants.MaxDisplayingErrorsNumber)
                                _errors.Add(new ReadFromFileError(currentRowIndex, supplier.ConversionErrorDescription));
                        }
                        currentRowIndex++;
                    }
                }
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
                _filteredBooks = booksFromDB.Where(book => (book.FirstName == firstName || firstNameCheck.IsChecked!=true) &&
                (book.LastName == lastName || lastNameCheck.IsChecked != true) &&
                (book.Surname == surname || surnameCheck.IsChecked != true) &&
                (book.BirthDate == correctBirthDate || birthDateCheck.IsChecked != true) &&
                (book.BookName == bookName || bookNameCheck.IsChecked != true) &&
                (book.BookYear == correctBookYear || bookYearCheck.IsChecked != true)).ToList();
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
        }

        private void Load_To_XML_Button_Click(object sender, RoutedEventArgs e)
        {
            const string userFilesDirectoryPath = FileConstants.UserFilesDirectoryPath + FileConstants.UserFilesDirectoryName;
            try
            {
                if (!Directory.Exists(userFilesDirectoryPath))
                    Directory.CreateDirectory(userFilesDirectoryPath);
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
            const string xmlFilePath = userFilesDirectoryPath + pathSeparator + FileConstants.XmlFileName;
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
