using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace MikhaleuLibrary.Model.DBModels
{
    
    /// <summary>
    ///  This class provides required game info separation to be saved further to database 
    /// </summary>
    public class Book
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key]
        public int Id { get; private set; }
        /// <summary>The game identifier.</summary>
        public string FirstName { get; private set; }

        /// <summary>The game end time.</summary>
        public string Surname { get; private set; }

        /// <summary>The game start time.</summary>
        public string LastName { get; private set; }

        /// <summary>The first player's symbol.</summary>
        public DateTime BirthDate { get; private set; }

        /// <summary>The second player's symbol.</summary>
        public string BookName { get; private set; }

        /// <summary>The game winner id or null if the game was drawn.</summary>
        public int BookYear { get; private set; }

        /// <summary>Initializes a new instance of the <see cref="GameDataForDB"/> class.</summary>
        public Book(string firstName,
            string surname,
            string lastName,
            DateTime birthDate,
            string bookName,
            int bookYear)
        {
            FirstName = firstName;
            Surname = surname;
            LastName = lastName;
            BirthDate = birthDate;
            BookName = bookName;
            BookYear = bookYear;
        }
    }
}
