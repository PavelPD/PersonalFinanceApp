using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PersonalFinanceApp.Models;

namespace PersonalFinanceApp.Data.Repositories
{
    public interface ITransactionRepository
    {
        Task<List<Transaction>> GetAllTransactions();
        Task<Transaction> GetTransactionById(int id);
        Task AddTransaction(Transaction transaction);
        Task UpdateTransaction(Transaction transaction);
        Task DeleteTransaction(int id);
    }
}
