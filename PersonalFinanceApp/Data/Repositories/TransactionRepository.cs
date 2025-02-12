using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PersonalFinanceApp.Models;
using SQLite;

namespace PersonalFinanceApp.Data.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly SQLiteAsyncConnection _database;

        public TransactionRepository()
        {
            _database = new SQLiteAsyncConnection(DBInitializer.DataBasePath);
        }

        public async Task<List<Transaction>> GetAllTransactions()
        {
            try
            {
                return await _database.QueryAsync<Transaction>("SELECT * FROM Transactions ORDER BY date DESC");
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при получении транзакций", ex);
            }
        }

        public async Task<Transaction> GetTransactionById(int id)
        {
            try
            {
                return await _database.FindAsync<Transaction>(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при получении транзакции с ID {id}", ex);
            }
        }

        public async Task AddTransaction(Transaction transaction)
        {
            try
            {
                await _database.ExecuteAsync(@"
                    INSERT INTO Transactions (type, amount, account_id, category_id, comment, date)
                    VALUES (?, ?, ?, ?, ?, ?)",
                    transaction.Type, transaction.Amount, transaction.AccountId,
                    transaction.CategoryId, transaction.Comment, transaction.Date);
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при добавлении транзакции", ex);
            }
        }

        public async Task UpdateTransaction(Transaction transaction)
        {
            try
            {
                await _database.ExecuteAsync(@"
                    UPDATE Transactions
                    SET type = ?, amount = ?, account_id = ?, category_id = ?, comment = ?, date = ?
                    WHERE id = ?",
                    transaction.Type, transaction.Amount, transaction.AccountId,
                    transaction.CategoryId, transaction.Comment, transaction.Date, transaction.Id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при обновлении транзакции с ID {transaction.Id}", ex);
            }
        }

        public async Task DeleteTransaction(int id)
        {
            try
            {
                await _database.ExecuteAsync("DELETE FROM Transaction WHERE id = ?", id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при удалении транзакции с ID {id}", ex);
            }
        }
    }
}
