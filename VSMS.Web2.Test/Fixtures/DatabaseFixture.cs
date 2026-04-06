using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using VSMS.Web2.Data;

namespace VSMS.Web2.Test.Fixtures
{
    /// <summary>
    /// Provides a fresh in-memory SQLite AppDbContext for each test class.
    /// Implements IDisposable to clean up database connections after tests complete.
    /// </summary>
    public class DatabaseFixture : IDisposable
    {
        private readonly SqliteConnection _connection;

        public AppDbContext Context { get; }

        public DatabaseFixture()
        {
            // Use an in-memory SQLite database (connection stays open for the lifetime of tests)
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(_connection)
                .Options;

            Context = new AppDbContext(options);
            Context.Database.EnsureCreated();
        }

        /// <summary>
        /// Creates a NEW independent context sharing the same in-memory database.
        /// Useful for simulating a "fresh read" after writes.
        /// </summary>
        public AppDbContext CreateNewContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(_connection)
                .Options;

            return new AppDbContext(options);
        }

        public void Dispose()
        {
            Context.Dispose();
            _connection.Close();
            _connection.Dispose();
        }
    }
}
