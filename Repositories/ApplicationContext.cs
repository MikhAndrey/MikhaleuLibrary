using Microsoft.EntityFrameworkCore;
using MikhaleuLibrary.Model.DBModels;
using MikhaleuLibrary.Constants;
using System.Configuration;

namespace MikhaleuLibrary.Repositories
{
    /// <summary>
    ///   This class provides database configuration params (i.e. database name, table names, field params etc.)
    /// </summary>
    public class ApplicationContext : DbContext
    {
        
        /// <summary>Value of connection string's name attribute.</summary>
        public string _connectionStringName;

        /// <summary>Table with all games.</summary>
        public DbSet<Book> Books { get; set; }

        /// <summary>Initializes <see cref="ApplicationContext"/> object. 
        /// Makes some basic actions like to make sure that database is created and sets connection string property.</summary>
        public ApplicationContext(string connectionStringName = ConnectionConstants.ConnectionStringAlias) {
            _connectionStringName = connectionStringName;   
            Database.EnsureCreated();
        }

        /// <summary>Sets some general config params of database we want to use.</summary>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = ConfigurationManager.ConnectionStrings[_connectionStringName].ConnectionString; 
            optionsBuilder.UseSqlServer(connectionString);
        }

        /// <summary>Specifies DB model params.</summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book>().Property("Id").ValueGeneratedOnAdd();
        }
    }
}
