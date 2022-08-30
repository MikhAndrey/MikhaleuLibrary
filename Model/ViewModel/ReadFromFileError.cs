using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MikhaleuLibrary.Model.ViewModel
{
    public class ReadFromFileError
    {
        public int RowNumber { get; set; }  
        public string ErrorDescription { get; set; }    
        public ReadFromFileError(int rowNumber, string errorDescription)
        {
            RowNumber = rowNumber;
            ErrorDescription = errorDescription;
        }

    }
}
