using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalFinanceApp.Data.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly SQLiteAsyncConnection _database;

        public AccountRepository()
        {           
            _database = new SQLiteAsyncConnection(DBInitializer.DataBasePath);
        }

        public async Task<List<Account>> GetAllAccount()
        {
            try
            {
                return await _database.QueryAsync<Account>("SELECT * FROM Accounts");
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при получении аккаунтов", ex);
            }
        }

        public async Task<Account> GetAccountById(int id)
        {
            try
            {
                return await _database.FindAsync<Account>(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при получении аккаунта с ID {id}", ex);
            }
        }

        public async Task AddAccount(Account account)
        {
            try
            {
                await _database.ExecuteAsync("INSERT INTO Accounts (name, balance) VALUES (?, ?)", 
                    account.Name, account.Balance);
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при добавлении аккаунта", ex);
            }
        }

        public async Task UpdateAccount(Account account)
        {
            try
            {
                await _database.ExecuteAsync("UPDATE Accounts SET name = ?, balance = ? WHERE id = ?",
                    account.Name, account.Balance, account.Id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при обновлении аккаунта с ID {account.Id}", ex);
            }
        }

        public async Task DeleteAccount(int id)
        {
            try
            {
                await _database.ExecuteAsync("DELETE FROM Accounts WHERE id = ?", id);
            }
            catch (Exception ex)
            {
                throw new Exception("$Ошибка при удалении аккаунта с ID {id}", ex);
            }
        }
    }
}
