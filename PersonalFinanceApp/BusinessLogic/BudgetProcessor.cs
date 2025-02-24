using PersonalFinanceApp.Data.Repositories;
using PersonalFinanceApp.Models;

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

        public async Task<List<BudgetsViewModel>> GetAllBudgetWithSpent()
        {
            var currentMonth = DateTime.Now.Month;

            var budgets = await _budgetRepository.GetAllBudgets();
            var transactionsForCurrentMonth = (await _transactionRepository.GetAllTransactions())
                .Where(t => t.Date.Month == currentMonth && t.Type == "expense");

            var result = new List<BudgetsViewModel>();

            foreach ( var budget in budgets )
            {
                var category = await _categoryRepository.GetCategoryById(budget.Category_id);
                var spent = transactionsForCurrentMonth.Where(t => t.Category_id == category.Id).Sum(t => t.Amount);

                result.Add(new BudgetsViewModel 
                {
                    Budgets_id = budget.Id,
                    Name = budget.Name,
                    Icon = category.Icon,
                    Category_id = category.Id,
                    Amount = budget.Amount,
                    Spent = spent
                });
            }

            return result;
        }

        public async Task<string> AddBudget(Budget budget)
        {
            if (budget.Amount <= 0) return "Сумма бюджета должна быть больше 0";

            var categury = await _categoryRepository.GetCategoryById(budget.Category_id);
            if(categury.Type != "expense")
            {
                return "Ошибка бюджет можно назначить только на категорию расходов.";
            }

            await _budgetRepository.AddBudget(budget);
            return "OK";
        }

        public async Task<string> UpdateBudget(Budget budget)
        {
            var existingBudget = await _budgetRepository.GetBudgetById(budget.Id);
            if (existingBudget == null)
            {
                return "Ошибка: бюджет не найден.";
            }

            if (budget.Amount <= 0) return "Сумма бюджета должна быть больше 0";

            var categury = await _categoryRepository.GetCategoryById(budget.Category_id);
            if (categury.Type != "expense")
            {
                return "Ошибка: бюджет можно назначить только на категорию расходов.";
            }

            await _budgetRepository.UpdateBudget(budget);
            return "OK";
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
