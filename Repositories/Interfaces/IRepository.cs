using System;
using System.Collections.Generic;
namespace MikhaleuLibrary.Repositories.Interfaces
{
    
    /// <summary>
    ///   This interface provides all necessary operations with data from databases.
    /// </summary>
    /// <typeparam name="T">Type of data stored in the database</typeparam>
    public interface IRepository<T> : IDisposable
        where T : class
    {
        
        /// <summary>Gets all DB records as a list.</summary>
        /// <returns>List with all db records</returns>
        IEnumerable<T> GetAll();

        /// <summary>Gets the item from DB by its ID.</summary>
        /// /// <param name="id">The identifier.</param>
        /// <returns>Item from DB with required ID.</returns>
        T? GetItem(int id);

        /// <summary>Adds the specified item.</summary>      
        /// <param name="item">Specified item we want to add to DB</param>
        void Add(T item);

        /// <summary>Updates the info about the specified item in DB.</summary>
        /// <param name="item">Specified item we want to update in DB.</param>
        void Update(T item);

        /// <summary>Deletes the item with the specified identifier.</summary>
        /// <param name="id">Id of item we want to delete</param>
        void Delete(int id);

        /// <summary>Saves DB changes.</summary>
        void Save();
    }
}
