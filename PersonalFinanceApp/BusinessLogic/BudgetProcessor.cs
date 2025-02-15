using PersonalFinanceApp.Data.Repositories;
using PersonalFinanceApp.Models;
using SQLitePCL;

namespace PersonalFinanceApp.BusinessLogic
{
    public class BudgetProcessor
    {
        private readonly IBudgetRepository _budgetRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ITransactionRepository _transactionRepository;

        public BudgetProcessor(IBudgetRepository budgetRepository, ICategoryRepository categoryRepository, ITransactionRepository transactionRepository)
        {
            _budgetRepository = budgetRepository;
            _categoryRepository = categoryRepository;
            _transactionRepository = transactionRepository;
        }

        public async Task<List<Budget>> GetAllBudget()
        {
            return await _budgetRepository.GetAllBudgets();
        }

        public async Task<string> AddBudget(Budget budget)
        {
            if (budget.Amount <= 0) return "Сумма бюджета должна быть больше 0.";

            var categury = await _categoryRepository.GetCategoryById(budget.Category_id);
            if(categury.Type != "expense")
            {
                return "Ошибка бюджет можно назначить только на категорию расходов.";
            }

            //Расчет потраченных средсв по категории
            budget.Spent = (await _transactionRepository.GetAllTransactions())
                .Where(t => t.Category_id == budget.Category_id)
                .Sum(t => t.Amount);

            budget.Month = DateTime.Now.Month;
            budget.Year = DateTime.Now.Year;

            await _budgetRepository.AddBudget(budget);
            return "Бюджет добавлен.";
        }

        public async Task<string> UpdateBudget(Budget budget)
        {
            var existingBudget = await _budgetRepository.GetBudgetById(budget.Id);
            if (existingBudget == null)
            {
                return "Ошибка: бюджет не найден.";
            }

            var categury = await _categoryRepository.GetCategoryById(budget.Category_id);
            if (categury.Type != "expense")
            {
                return "Ошибка: бюджет можно назначить только на категорию расходов.";
            }

            //если измениласт категория пересчитываем spent
            if (existingBudget.Category_id != budget.Category_id)
            {
                budget.Spent = (await _transactionRepository.GetAllTransactions())
                    .Where(t => t.Category_id == budget.Category_id)
                    .Sum(t => t.Amount);
            }

            budget.Month = DateTime.Now.Month;
            budget.Year = DateTime.Now.Year;

            await _budgetRepository.UpdateBudget(budget);
            return "Бюджет обновлен.";
        }

        public async Task<string> DeleteBudget(int id)
        {
            var budget = await _budgetRepository.GetBudgetById(id);
            if (budget == null)
            {
                return "Ошибка: бюджет не найден.";
            }
            await _budgetRepository.DeleteBudget(id);
            return "Бюджет удален.";
        }
    }
}
