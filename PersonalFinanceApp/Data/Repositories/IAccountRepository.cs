using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalFinanceApp.Data.Repositories
{
    public interface IAccountRepository
    {
        Task<List<Account>> GetAllAccount();
        Task<Account> GetAccountById(int id);
        Task AddAccount(Account account);
        Task UpdateAccount(Account account);
        Task DeleteAccount(int id);     
    }
}
