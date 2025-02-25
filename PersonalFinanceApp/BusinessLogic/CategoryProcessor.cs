using PersonalFinanceApp.Data.Repositories;
using PersonalFinanceApp.Models;

namespace PersonalFinanceApp.BusinessLogic
{
    public class CategoryProcessor
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IAccountRepository _accountRepository;

        public CategoryProcessor(ICategoryRepository categoryRepository, ITransactionRepository transactionRepository, IAccountRepository accountRepository)
        {
            _categoryRepository = categoryRepository;
            _transactionRepository = transactionRepository;
            _accountRepository = accountRepository;
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
                return "Имя категории не может быть пустым";
            }

            await _categoryRepository.AddCategory(category);
            return "OK";
        }

        public async Task<string> UpdateCategory(Category category)
        {
            if (string.IsNullOrWhiteSpace(category.Name)) return "Введите название категории";            

            var existingCategory = await _categoryRepository.GetCategoryById(category.Id);
            if (existingCategory == null)
            {
                return "Ошибка: категория не найдена.";
            }

            //меняем только name и icon
            existingCategory.Name = category.Name;
            existingCategory.Icon = category.Icon;

            await _categoryRepository.UpdateCategory(existingCategory);
            return "OK";
        }

        public async Task<string> DeleteCategory(int id)
        {
            var category = await _categoryRepository.GetCategoryById(id);
            if (category == null)
            {
                return "Ошибка: категория не найдена.";
            }

            //получаем все транзакции с этой категорией
            var transactions = (await _transactionRepository.GetAllTransactions())
                .Where(t => t.Category_id == id)
                .ToList();
            if (transactions.Any())
            {
                foreach (var transaction in transactions)
                {
                    //возвращаем сумму на счет
                    var account = await _accountRepository.GetAccountById(transaction.Account_id);
                    account.Balance += transaction.Type == "income" ? -transaction.Amount : transaction.Amount;
                    await _accountRepository.UpdateAccount(account);
                }
            }

            await _categoryRepository.DeleteCategory(id);
            return "OK";
        }
    }
}
