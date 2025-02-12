using PersonalFinanceApp.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalFinanceApp.Data.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly SQLiteAsyncConnection _database;

        public CategoryRepository()
        {
            _database = new SQLiteAsyncConnection(DBInitializer.DataBasePath);
        }

        public async Task<List<Category>> GetAllCategories()
        {
            try
            {
                return await _database.QueryAsync<Category>("SELECT * FROM Categories");
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при получении категорий", ex);
            }
        }

        public async Task<Category> GetCategoryById(int id)
        {
            try
            {
                return await _database.FindAsync<Category>(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при получении категории с ID {id}", ex);
            }
        }

        public async Task AddCategory(Category category)
        {
            try
            {
                await _database.ExecuteAsync("INSERT INTO Categories (name, type, icon) VALUES (?, ?, ?)",
                    category.Name, category.Type, category.Icon);
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при добавлении категории", ex);
            }
        }

        public async Task UpdateCategory(Category category)
        {
            try
            {
                await _database.ExecuteAsync("UPDATE Categories SET name = ?, type = ?, icon = ? WHERE id = ?",
                    category.Name, category.Type, category.Icon, category.Id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при обновлении категории с ID {category.Id}", ex);
            }
        }

        public async Task DeleteCategory(int id)
        {
            try
            {
                await _database.ExecuteAsync("DELETE FROM Categories WHERE id = ?", id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при удалении категории с ID {id}", ex);
            }
        }
    }
}
