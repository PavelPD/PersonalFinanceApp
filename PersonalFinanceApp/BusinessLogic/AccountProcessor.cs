using PersonalFinanceApp.Data.Repositories;
using PersonalFinanceApp.Models;

namespace PersonalFinanceApp.BusinessLogic
{
    public class AccountProcessor
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IBudgetRepository _budgetRepository;

        public AccountProcessor(IAccountRepository accountRepository, ITransactionRepository transactionRepository, IBudgetRepository budgetRepository) 
        {
            _accountRepository = accountRepository;
            _transactionRepository = transactionRepository;
            _budgetRepository = budgetRepository;
        }

        public async Task<List<Account>> GetAllAccounts()
        {
            return await _accountRepository.GetAllAccounts();
        }

        public async Task<string> AddAccount(Account account)
        {
            //проверка на пустое имя
            if (string.IsNullOrWhiteSpace(account.Name)) 
            {
                return "Ошибка: имя счета не может быть пустым.";
            }

            if(account.Balance < 0)
            {
                return "Ошибка: баланс счета не может быть отрицательным.";
            }

            await _accountRepository.AddAccount(account);
            return "Счет добавлен.";
        }

        public async Task<string> UpdateAccount(Account account)
        {
            var existingAccount = await _accountRepository.GetAccountById(account.Id);
            if (existingAccount == null)
            {
                return "Ошибка: счет не найден.";
            }

            await _accountRepository.UpdateAccount(account);
            return "Счет обновлен.";
        }

        public async Task<string> DeleteAccount(int id)
        {
            var account = await _accountRepository.GetAccountById(id);
            if (account == null)
            {
                return "Ошибка: счет не найден.";
            }

            var transactions = await _transactionRepository.GetExpenseTransactionByAccountId(id);


            if (transactions.Any())
            {
                var budgets = await _budgetRepository.GetAllBudgets();

                foreach (var transaction in transactions) 
                {
                    foreach (var budget in budgets.Where(b => b.Category_id == transaction.Category_id))
                    {
                        budget.Spent -= transaction.Amount;
                        if (budget.Spent < 0) budget.Spent = 0;
                        await _budgetRepository.UpdateBudget(budget);
                    }
                }
            }

            await _accountRepository.DeleteAccount(id);
            return "Счет удален.";
        }
    }
}
