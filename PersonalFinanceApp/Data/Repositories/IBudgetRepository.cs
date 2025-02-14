using PersonalFinanceApp.Models;

namespace PersonalFinanceApp.Data.Repositories
{
    public interface IBudgetRepository
    {
        Task<List<Budget>> GetAllBudgets();
        Task<Budget> GetBudgetById(int id);
        Task AddBudget(Budget budget);
        Task UpdateBudget(Budget budget);
        Task DeleteBudget(int id);
    }
}
