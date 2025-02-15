using PersonalFinanceApp.Data.Repositories;
using PersonalFinanceApp.Models;


namespace PersonalFinanceApp.BusinessLogic
{
    public class TransactionProcessor
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IBudgetRepository _budgetRepository;

        public TransactionProcessor(ITransactionRepository transactionRepository, ICategoryRepository categoryRepository,
            IAccountRepository accountRepository, IBudgetRepository budgetRepository)
        {
            _transactionRepository = transactionRepository;
            _categoryRepository = categoryRepository;
            _accountRepository = accountRepository;
            _budgetRepository = budgetRepository;
        }

        public async Task<List<Transaction>> GetAllTransaction()
        {
            try
            {
                return await _transactionRepository.GetAllTransactions();
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при получении транзакций", ex);
            }
        }

        public async Task<string> AddTransaction(Transaction transaction)
        {
            if(transaction.Amount <= 0)
            {
                return "Ошибка: сумма транзакции должна быть больше 0.";
            }
                        
            var category = await _categoryRepository.GetCategoryById(transaction.Category_id);
            if (category == null)
            {
                return "Указанной категории не существует";
            }

            var account = await _accountRepository.GetAccountById(transaction.Account_id);
            if (account == null)
            {
                return "Указанного счета не существует";
            }

            //обновляем бюджеты
            await UpdateBudgetsOnTransactionAdded(transaction);

            //добавляем транзакцию в бд
            await _transactionRepository.AddTransaction(transaction);

            //обновляем баланс
            await UpdateAccountBalance(account, transaction.Amount, transaction.Type == "income");

            return "Транзакция добавлена.";
        }

        public async Task<string> UpdateTransaction(Transaction transaction)
        {
            var categoryType = await _categoryRepository.GetCategoryById(transaction.Category_id);
            if (categoryType.Type != transaction.Type)
            {
                return "Ошибка: тип категории не соответствует типу транзакции.";
            }

            var existingTransaction = await _transactionRepository.GetTransactionById(transaction.Id);
            if (existingTransaction == null)
            {
                return "Ошибка: транзакция не найдена.";
            }

            //оставляем тип оригинала
            transaction.Type = existingTransaction.Type;

            //обновляем бюджеты
            await UpdateBudgetsOnTransactionUpdated(existingTransaction, transaction);

            //обновляем счета
            await UpdateAccountOnTransactionUpdated(existingTransaction ,transaction);

            //оюновляем в бд
            await _transactionRepository.UpdateTransaction(transaction);
            
            return "Транзакция обновлена";
        }

        public async Task<string> DeleteTransaction(int id)
        {
            var transaction = await _transactionRepository.GetTransactionById(id);
            if (transaction == null)
            {
                return "Ошибка: транзакция не найдена.";
            }

            //обновляем бюджеты
            await UpdateBudgetsOnTransactionDeleted(transaction);
            
            var account = await _accountRepository.GetAccountById(transaction.Account_id);
            if (account != null)
            {
                //возвращаем сумму на баланс
                await UpdateAccountBalance(account, -transaction.Amount, transaction.Type == "income");
            }

            //удаляеь в бд
            await _transactionRepository.DeleteTransaction(id);
            return "Транзакция удалена.";
        }

        //метод для обновления баланса
        private async Task UpdateAccountBalance(Account account, double amount, bool isIncome)
        {
            account.Balance += isIncome ? amount : -amount;
            await _accountRepository.UpdateAccount(account);
        }

        //метод для обновления бюджетов при добавлении транзакции
        private async Task UpdateBudgetsOnTransactionAdded(Transaction transaction)
        {
            var budgets = (await _budgetRepository.GetAllBudgets())
                .Where(b => b.Category_id == transaction.Category_id)
                .ToList();

            if (budgets.Any())
            {
                foreach (var budget in budgets)
                {
                    budget.Spent += transaction.Amount;
                    await _budgetRepository.UpdateBudget(budget);
                }
            }
        }

        //метод для обновления бюджетов при изменении транзакции
        private async Task UpdateBudgetsOnTransactionUpdated(Transaction oldTransaction, Transaction newTransaction)
        {
            double difference = newTransaction.Amount - oldTransaction.Amount;

            if (difference != 0)
            {

                var budgets = (await _budgetRepository.GetAllBudgets())
                    .Where(b => b.Category_id == newTransaction.Category_id)
                    .ToList();

                if (budgets.Any())
                {
                    foreach (var budget in budgets)
                    {
                        budget.Spent += difference;
                        if (budget.Spent < 0) budget.Spent = 0;
                        await _budgetRepository.UpdateBudget(budget);
                    }
                }
            }

            if (oldTransaction.Category_id != newTransaction.Category_id)
            {
                await MoveTransactionBetweenBudgets(oldTransaction, newTransaction);
            }                    
        }

        //метод для переноса суммы между бюджетами при изменении категории транзакции
        private async Task MoveTransactionBetweenBudgets(Transaction oldTransaction, Transaction newTransaction)
        {
            var oldBudgets = (await _budgetRepository.GetAllBudgets())
                    .Where(b => b.Category_id == oldTransaction.Category_id)
                    .ToList();

            foreach (var budget in oldBudgets)
            {
                budget.Spent -= oldTransaction.Amount;
                if (budget.Spent < 0) budget.Spent = 0;
                await _budgetRepository.UpdateBudget(budget);
            }

            var newBudgets = (await _budgetRepository.GetAllBudgets())
                .Where(b => b.Category_id == newTransaction.Category_id)
                .ToList();

            foreach (var budget in newBudgets)
            {
                budget.Spent += newTransaction.Amount;
                await _budgetRepository.UpdateBudget(budget);
            }
        }

        //метод обновления бюджетов при удалении транзакции
        private async Task UpdateBudgetsOnTransactionDeleted(Transaction transaction)
        {
            var budgets = (await _budgetRepository.GetAllBudgets())
                    .Where(b => b.Category_id == transaction.Category_id)
                    .ToList();
            foreach(var budget in budgets)
            {
                budget.Spent -= transaction.Amount;
                if (budget.Spent < 0) budget.Spent = 0;
                await _budgetRepository.UpdateBudget(budget);
            }
        }

        //метод обновления счетов при изменении транзакции
        private async Task UpdateAccountOnTransactionUpdated(Transaction oldTransaction, Transaction newTransaction)
        {
            if (oldTransaction.Account_id != newTransaction.Account_id)
            {
                var oldAccount = await _accountRepository.GetAccountById(oldTransaction.Account_id);
                var newAccount = await _accountRepository.GetAccountById(newTransaction.Account_id);

                if(oldAccount != null)
                {
                    await UpdateAccountBalance(oldAccount, -oldTransaction.Amount, oldTransaction.Type == "income");
                }

                if (newAccount != null)
                {
                    await UpdateAccountBalance(newAccount, newTransaction.Amount, newTransaction.Type == "income");
                }
            }

            else if (oldTransaction.Amount != newTransaction.Amount)
            {
                var account = await _accountRepository.GetAccountById(newTransaction.Account_id);
                double difference = newTransaction.Amount - oldTransaction.Amount;
                await UpdateAccountBalance(account, difference, newTransaction.Type == "income");
            }
        }
    }
}
