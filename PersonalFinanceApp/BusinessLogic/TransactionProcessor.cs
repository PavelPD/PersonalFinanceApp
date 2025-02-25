using PersonalFinanceApp.Data.Repositories;
using PersonalFinanceApp.Models;


namespace PersonalFinanceApp.BusinessLogic
{
    public class TransactionProcessor
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IAccountRepository _accountRepository;

        public TransactionProcessor(ITransactionRepository transactionRepository, ICategoryRepository categoryRepository,
            IAccountRepository accountRepository)
        {
            _transactionRepository = transactionRepository;
            _categoryRepository = categoryRepository;
            _accountRepository = accountRepository;
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

        public async Task<List<TransactionViewModel>> GetAllTransactionViewModel(DateTime date)
        {
            try
            {
                var transactoins = (await _transactionRepository.GetAllTransactions())
                    .Where(t => t.Date.ToString("yyyy MMMM") == date.ToString("yyyy MMMM"));

                var result = new List<TransactionViewModel>();

                foreach (var transaction in transactoins)
                {
                    var category = await _categoryRepository.GetCategoryById(transaction.Category_id);
                    var account = await _accountRepository.GetAccountById(transaction.Account_id);

                    result.Add(new TransactionViewModel
                    {
                        Transaction_id = transaction.Id,
                        Type = transaction.Type,
                        Amount = transaction.Amount,
                        Comment = transaction.Comment,
                        Date = transaction.Date,
                        CategoryName = category.Name,
                        CategoryIcon = category.Icon,
                        AccountName = account.Name,
                        AmountColor = transaction.Type == "income" ? "Green" : "#272727"                        
                    });
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при получении транзакций", ex);
            }
        }

        public async Task<string> AddTransaction(Transaction transaction)
        {
            if(transaction.Amount < 0) return "Cумма не может быть отрицательной";
            
                        
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

            //добавляем транзакцию в бд
            await _transactionRepository.AddTransaction(transaction);

            //обновляем баланс
            await UpdateAccountBalance(account, transaction.Amount, transaction.Type == "income");

            return "OK";
        }

        public async Task<string> UpdateTransaction(Transaction transaction)
        {
            if (transaction.Amount < 0) return "Сумма не может быть отрицательной";
            if (!isNumeric(transaction.Amount)) return "Сумма должна сожержать тольо цифры";

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
            
            //обновляем счета
            await UpdateAccountOnTransactionUpdated(existingTransaction ,transaction);

            //обновляем в бд
            await _transactionRepository.UpdateTransaction(transaction);
            
            return "OK";
        }

        public async Task<string> DeleteTransaction(int id)
        {
            var transaction = await _transactionRepository.GetTransactionById(id);
            if (transaction == null)
            {
                return "Ошибка: транзакция не найдена.";
            }
            
            var account = await _accountRepository.GetAccountById(transaction.Account_id);
            if (account != null)
            {
                //возвращаем сумму на баланс
                await UpdateAccountBalance(account, -transaction.Amount, transaction.Type == "income");
            }

            //удаляеь в бд
            await _transactionRepository.DeleteTransaction(id);
            return "OK";
        }

        //метод для обновления баланса
        private async Task UpdateAccountBalance(Account account, double amount, bool isIncome)
        {
            account.Balance += isIncome ? amount : -amount;
            await _accountRepository.UpdateAccount(account);
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

        //проверка что сумма это число
        private bool isNumeric(object value)
        {
            return double.TryParse(value.ToString(), out _);
        }
    }
}
