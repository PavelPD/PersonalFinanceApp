using SQLite;

namespace PersonalFinanceApp.Data.Repositories
{
    public class DataBaseRepository : IDataBaseRepository
    {
        private readonly SQLiteAsyncConnection _database;

        public DataBaseRepository()
        {
            _database = new SQLiteAsyncConnection(DBInitializer.DataBasePath);
        }

        public async Task RestDataBase()
        {
            try
            {
                await _database.ExecuteAsync("DROP TABLE IF EXISTS Transactions");
                await _database.ExecuteAsync("DROP TABLE IF EXISTS Accounts");
                await _database.ExecuteAsync("DROP TABLE IF EXISTS Categories");
                await _database.ExecuteAsync("DROP TABLE IF EXISTS Budgets");

                await DBInitializer.InitializeDatabase();
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при удалении базы данных", ex);
            }
        }
    }
}
