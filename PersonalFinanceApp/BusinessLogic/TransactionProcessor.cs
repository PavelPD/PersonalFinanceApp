using PersonalFinanceApp.Data.Repositories;
using PersonalFinanceApp.Models;

namespace PersonalFinanceApp.BusinessLogic
{
    public class TransactionProcessor
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IAccountRepository _accountRepository;

        public TransactionProcessor(ITransactionRepository transactionRepository, ICategoryRepository categoryRepository, IAccountRepository accountRepository)
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

            await _transactionRepository.AddTransaction(transaction);

            //обновляем баланс
            account.Balance += transaction.Type == "income" ? transaction.Amount : -transaction.Amount; 
            await _accountRepository.UpdateAccount(account);

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

            //обновление счетов при смене счета транзакции и без
            if (existingTransaction.Account_id != transaction.Account_id)
            {
                var oldAccount = await _accountRepository.GetAccountById(existingTransaction.Account_id);
                var newAccount = await _accountRepository.GetAccountById(transaction.Account_id);

                //возвращаем сумму на старый счет
                oldAccount.Balance += existingTransaction.Type == "income" ? -existingTransaction.Amount : existingTransaction.Amount;
                await _accountRepository.UpdateAccount(oldAccount);

                //добавляем сумму на новый счет
                newAccount.Balance += transaction.Type == "income" ? transaction.Amount : -transaction.Amount;
                await _accountRepository.UpdateAccount(newAccount);
            }
            else if (existingTransaction.Account_id == transaction.Account_id)
            {
                var account = await _accountRepository.GetAccountById(transaction.Account_id);
                double difference = transaction.Amount - existingTransaction.Amount;
                account.Balance += transaction.Type == "income" ? difference : -difference;
                await _accountRepository.UpdateAccount(account);
            }

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

            var account = await _accountRepository.GetAccountById(transaction.Account_id);

            //возврат суммы на баланс 
            account.Balance += transaction.Type == "income" ? -transaction.Amount : transaction.Amount;
            await _accountRepository.UpdateAccount(account);

            await _transactionRepository.DeleteTransaction(id);
            return "Транзакция удалена.";
        }
    }
}
