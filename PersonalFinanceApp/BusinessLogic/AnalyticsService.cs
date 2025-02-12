using PersonalFinanceApp.Data.Repositories;
using PersonalFinanceApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalFinanceApp.BusinessLogic
{
    public class AnalyticsService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IAccountRepository _accountRepository;

        public AnalyticsService(ITransactionRepository transactionRepository, IAccountRepository accountRepository)
        {
            _transactionRepository = transactionRepository;
            _accountRepository = accountRepository;
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

        //Прирост средств за период с возможной фильтрацией
        public async Task<double> GetProfit(DateTime startDate, DateTime endDate, int? accountId = null, int? categoryId = null)
        {
            var transactoins = await _transactionRepository.GetAllTransactions();
            
            //фильтр по дате
            var filteredTransactoins = transactoins
                .Where(t => t.Date >= startDate && t.Date <= endDate);
            
            //если выбран счет, фильтруем по нему
            if (accountId.HasValue)
            {
                filteredTransactoins = filteredTransactoins
                    .Where(t => t.AccountId == accountId.Value);
            }

            //доходы
            double income = filteredTransactoins
                .Where(t => t.Type == "income")
                .Sum(t => t.Amount);

            //расходы (если выбрана категория, фильтруем)
            var expenseTransactoins = filteredTransactoins.Where(t => t.Type == "expense");
            if (categoryId.HasValue)
            {
                expenseTransactoins = expenseTransactoins
                    .Where(t => t.CategoryId == categoryId.Value);
            }

            double expense = expenseTransactoins.Sum(t => t.Amount);

            return income - expense;
        }

        //Группировка доходов и расходов по месяцам за последние N месяцев
        public async Task<List<(string Month, double Total)>> GetMonthlyTransactions(int monthsBack, string filterType)
        {
            var transactoins = await _transactionRepository.GetAllTransactions();

            //фильтр по периоду и типу
            var filteredTransactoins = transactoins
                .Where(t => t.Date >= DateTime.Now.AddMonths(-monthsBack))
                .Where(t => t.Type == filterType);           

            //группировка данных по месяцам
            var result = filteredTransactoins
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
        public async Task<Dictionary<string, double>> GetCategoryExpenses(DateTime startDate, DateTime endDate, int? accountId = null)
        {
            var transactoins = await _transactionRepository.GetAllTransactions();

            var filteredTransactoins = transactoins
                .Where(t => t.Type == "expense" && t.Date >= startDate && t.Date <= endDate);

            if (accountId.HasValue)
            {
                filteredTransactoins = filteredTransactoins
                    .Where(t => t.AccountId == accountId.Value);
            }

            var result = filteredTransactoins
                .GroupBy(t => t.CategoryId)
                .Select(g => new { CategoryID = g.Key, TotalSpent = (double)g.Sum(t => t.Amount) })
                .ToDictionary(g => g.CategoryID.ToString(), g => g.TotalSpent);

            return result;
        }
    }
}
