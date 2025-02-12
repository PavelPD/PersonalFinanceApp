using PersonalFinanceApp.Data.Repositories;
using PersonalFinanceApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalFinanceApp.BusinessLogic
{
    public class BudgetProcessor
    {
        private readonly IBudgetRepository _budgetRepository;
        private readonly ICategoryRepository _categoryRepository;

        public BudgetProcessor(IBudgetRepository budgetRepository, ICategoryRepository categoryRepository)
        {
            _budgetRepository = budgetRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<List<Budget>> GetAllBudget()
        {
            return await _budgetRepository.GetAllBudgets();
        }

        public async Task<string> AddBudget(Budget budget)
        {
            var categury = await _categoryRepository.GetCategoryById(budget.CategoryId);
            if(categury.Type != "expense")
            {
                return "Ошибка6 бюджет можно назначить только на категорию расходов.";
            }

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

            var categury = await _categoryRepository.GetCategoryById(budget.CategoryId);
            if (categury.Type != "expense")
            {
                return "Ошибка6 бюджет можно назначить только на категорию расходов.";
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
