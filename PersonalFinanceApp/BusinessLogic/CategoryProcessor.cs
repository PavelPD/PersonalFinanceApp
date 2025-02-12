using PersonalFinanceApp.Data.Repositories;
using PersonalFinanceApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalFinanceApp.BusinessLogic
{
    public class CategoryProcessor
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryProcessor(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<List<Category>> GetAllCategory()
        {
            return await _categoryRepository.GetAllCategories();
        }

        public async Task<string> AddCategory(Category category)
        {
            //проверка на пустое имя
            if (string.IsNullOrWhiteSpace(category.Name)) 
            {
                return "Ошибка: имя категории не может быть пустым.";
            }

            await _categoryRepository.AddCategory(category);
            return "Категория добавлена.";
        }

        public async Task<string> UpdateCategoryt(Category category)
        {
            var existingCategory = await _categoryRepository.GetCategoryById(category.Id);
            if (existingCategory == null)
            {
                return "Ошибка: категория не найдена.";
            }

            //меняем только name и icon
            existingCategory.Name = category.Name;
            existingCategory.Icon = category.Icon;

            await _categoryRepository.UpdateCategory(existingCategory);
            return "Категория обновлена.";
        }

        public async Task<string> DeleteCategory(int id)
        {
            var account = await _categoryRepository.GetCategoryById(id);
            if (account == null)
            {
                return "Ошибка: каттегория не найдена.";
            }

            await _categoryRepository.DeleteCategory(id);
            return "Категория удалена.";
        }
    }
}
