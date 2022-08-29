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
using System.Collections;

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
            _errorsGrid = new DataGrid();
            MainGrid.Children.Add(_errorsGrid);
            var column = new DataGridTextColumn();
            column.Header = "Row number";
            column.Binding = new Binding("_rowNumber");
            _errorsGrid.Columns.Add(column);
            column = new DataGridTextColumn();
            column.Header = "Error description";
            column.Binding = new Binding("_errorDescription");
            _errorsGrid.Columns.Add(column);
        }

        private DataGrid _errorsGrid;

        private string? _sourceFileName = null;

        private List<ReadFromFileError> _errors = new();

        private void FillErrorsGrid()
        {
            for (int i = 0; i < _errors.Count; i++)
            {
                _errorsGrid.Items.Add(_errors[i]);
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
            }
        }

        private async void Load_Button_Click(object sender, RoutedEventArgs e)
        {
            if (_sourceFileName != null)
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
                    loadToDBButton.IsEnabled = true;
                    return;
                }
                const string pathSeparator = "/";
                const string _errorsFilePath = userFilesDirectoryPath + pathSeparator + FileConstants.ErrorsFileName;
                FileToDBSupplier supplier = new();
                supplier.EndOfFileReached = false;
                using (StreamReader reader = new(_sourceFileName))
                {
                    using (StreamWriter writer = new(_errorsFilePath, false))
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
                                FillErrorsGrid();
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
                                    if (currentRowIndex < ViewConstants.MaxDisplayingErrorsNumber) {
                                        _errors.Add(new ReadFromFileError(currentRowIndex, supplier.ConversionErrorDescription));
                                    }
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
                            }
                            currentRowIndex++;
                        }
                    }
                }
            } else
            {
                MessageBox.Show(Messages.WrongClicksOrderMessage,
                    Messages.ReadFromFileErrorDescription,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}
