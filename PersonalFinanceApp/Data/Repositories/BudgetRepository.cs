﻿using PersonalFinanceApp.Models;
using SQLite;

namespace PersonalFinanceApp.Data.Repositories
{
    public class BudgetRepository : IBudgetRepository
    {
        private readonly SQLiteAsyncConnection _database;

        public BudgetRepository()
        {
            _database = new SQLiteAsyncConnection(DBInitializer.DataBasePath);
        }

        public async Task<List<Budget>> GetAllBudgets()
        {
            try
            {
                return await _database.QueryAsync<Budget>("SELECT * FROM Budgets");
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при получении бюджетов", ex);
            }
        }

        public async Task<Budget?> GetBudgetById(int id)
        {
            try
            {
                var result = await _database.QueryAsync<Budget>("SELECT * FROM Budgets WHERE id = ?", id);
                return result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при получении бюджета с ID {id}", ex);
            }
        }

        public async Task AddBudget(Budget budget)
        {
            try
            {
                await _database.ExecuteAsync("INSERT INTO Budgets (name, category_id, amount) VALUES (?, ?, ?)",
                    budget.Name, budget.Category_id, budget.Amount);
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при добавлении бюджета", ex);
            }
        }

        public async Task UpdateBudget(Budget budget)
        {
            try
            {
                await _database.ExecuteAsync("UPDATE Budgets SET name = ?, category_id = ?, amount = ? WHERE id = ?",
                    budget.Name, budget.Category_id, budget.Amount, budget.Id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при обновлении бюджета с ID {budget.Id}", ex);
            }
        }

        public async Task DeleteBudget(int id)
        {
            try
            {
                await _database.ExecuteAsync("DELETE FROM Budgets WHERE id = ?", id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при удалении бюджета с ID {id}", ex);
            }
        }
    }
}
