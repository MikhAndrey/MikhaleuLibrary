using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MikhaleuLibrary.Model.ViewModel
{
    public class ReadFromFileError
    {
        public int _rowNumber;
        public string _errorDescription;
        public ReadFromFileError(int rowNumber, string errorDescription)        {
            _rowNumber = rowNumber;
            _errorDescription = errorDescription;
        }

    }
}
