using PersonalFinanceApp.Data.Repositories;
using PersonalFinanceApp.Models;

namespace PersonalFinanceApp.BusinessLogic
{
    public class AnalyticsService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ICategoryRepository _categoryRepository;

        public AnalyticsService(ITransactionRepository transactionRepository, IAccountRepository accountRepository, ICategoryRepository categoryRepository)
        {
            _transactionRepository = transactionRepository;
            _accountRepository = accountRepository;
            _categoryRepository = categoryRepository;
        }

        //Общий баланс
        public async Task<double> GetTotalBalance(int? accountId = null)
        {
            //фильтруем, если выбран счет
            if (accountId.HasValue)
            {
                var account = await _accountRepository.GetAccountById(accountId.Value);
                return account.Balance;
            }

            var accounts = await _accountRepository.GetAllAccounts();
            return accounts.Sum(a => a.Balance);
        }

        public async Task<double> GetTransactionsAmountForMonth(DateTime transMonth, bool isIncome)
        {
            var type = isIncome ? "income" : "expense";
            var result = (await _transactionRepository.GetAllTransactions())
                .Where(t => t.Date.ToString("yyyy MMMM") == transMonth.ToString("yyyy MMMM") && t.Type == type)
                .Sum(t => t.Amount);

            return result;
        }

        //Прирост средств за период с возможной фильтрацией
        public async Task<double> GetProfit(DateTime startDate, DateTime endDate, int? accountId = null, int? categoryId = null)
        {
            //фильтр по дате
            var transactoins = (await _transactionRepository.GetAllTransactions())
                .Where(t => t.Date >= startDate && t.Date <= endDate);               
            
            //если выбран счет, фильтруем по нему
            if (accountId.HasValue)
            {
                transactoins = transactoins
                    .Where(t => t.Account_id == accountId.Value);
            }

            //доходы
            double income = transactoins
                .Where(t => t.Type == "income")
                .Sum(t => t.Amount);

            //расходы (если выбрана категория, фильтруем)
            var expenseTransactoins = transactoins.Where(t => t.Type == "expense");
            if (categoryId.HasValue)
            {
                expenseTransactoins = expenseTransactoins
                    .Where(t => t.Category_id == categoryId.Value);
            }

            double expense = expenseTransactoins.Sum(t => t.Amount);

            return income - expense;
        }

        //Группировка доходов и расходов по месяцам за последние N месяцев
        public async Task<List<(string Month, double Total)>> GetMonthlyTransactions(int monthsBack, string filterType)
        {
            //фильтр по периоду и типу
            var transactoins = (await _transactionRepository.GetAllTransactions())
                .Where(t => t.Date >= DateTime.Now.AddMonths(-monthsBack))
                .Where(t => t.Type == filterType);           

            //группировка данных по месяцам
            var result = transactoins
                .GroupBy(t => new { t.Date.Year, t.Date.Month })
                .Select(g => new
                {
                    Month = $"{g.Key.Month}/{g.Key.Year}",
                    Total = (double)g.Sum(t => t.Amount) //сумма доходов или расходов
                })
                .OrderBy(g => g.Month)
                .Select(g => (g.Month, g.Total))
                .ToList();

            return result;
        }

        //Получить траты по категориям за период с возможной фильтрацией по счету
        public async Task<List<CategoryExpenseViewModel>> GetCategoryExpenses(DateTime startDate, DateTime endDate, bool isIncome, int? accountId = null)
        {
            var allMonthlyAmounts = await GetTransactionsAmountForMonth(startDate, isIncome);
            string type = isIncome ? "income" : "expense";

            //фильтр по дате
            var transactoins = (await _transactionRepository.GetAllTransactions())
                .Where(t => t.Type == type && t.Date >= startDate && t.Date <= endDate);

            if (accountId.HasValue)
            {
                transactoins = transactoins
                    .Where(t => t.Account_id == accountId.Value);
            }

            var groupedTransactions = transactoins
                .GroupBy(t => t.Category_id)
                .Select(g => new {Category_id = g.Key, TotalSpent = (double)g.Sum(t => t.Amount)})
                .OrderByDescending(g => g.TotalSpent)
                .ToList();

            var result = new List<CategoryExpenseViewModel>();

            foreach (var transaction in groupedTransactions)
            {
                var category = await _categoryRepository.GetCategoryById(transaction.Category_id);
                if(category != null)
                {
                    result.Add(new CategoryExpenseViewModel
                    {
                        Category_id = category.Id,
                        Name = category.Name,
                        Icon = category.Icon,
                        Amount = transaction.TotalSpent,
                        Percentage = Math.Round((transaction.TotalSpent / allMonthlyAmounts), 2)
                    });
                }
            }            

            return result;
        }
    }
}
