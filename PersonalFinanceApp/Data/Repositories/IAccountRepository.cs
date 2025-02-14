using PersonalFinanceApp.Models;

namespace PersonalFinanceApp.Data.Repositories
{
    public interface IAccountRepository
    {
        Task<List<Account>> GetAllAccounts();
        Task<Account> GetAccountById(int id);
        Task AddAccount(Account account);
        Task UpdateAccount(Account account);
        Task DeleteAccount(int id);     
    }
}
