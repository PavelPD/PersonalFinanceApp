using PersonalFinanceApp.BusinessLogic;
using PersonalFinanceApp.Data.Repositories;
using PersonalFinanceApp.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalFinanceApp
{
    public class TestConsole
    {
        private readonly TransactionProcessor _transactionProcessor;
        private readonly AccountProcessor _accountProcessor;
        private readonly CategoryProcessor _categoryProcessor;
        private readonly AnalyticsService _analyticsService;

        private static readonly string LogFile = @"C:\Users\Pavel\Desktop\test_log.txt";

        public TestConsole()
        {
            //репозитории
            var transactionRepos = new TransactionRepository();
            var accountRepos = new AccountRepository();
            var categoryRepos = new CategoryRepository();

            //процессоры
            _categoryProcessor = new CategoryProcessor(categoryRepos, transactionRepos, accountRepos);
            _transactionProcessor = new TransactionProcessor(transactionRepos, categoryRepos, accountRepos);
            _accountProcessor = new AccountProcessor(accountRepos);

            //аналитика
            _analyticsService = new AnalyticsService(transactionRepos, accountRepos);

            //чистим логи
            File.WriteAllText(LogFile, "LogFile\n");
        }        

        public async Task RunTest()
        {
            //Log("\n=== тестирование счетов ===");
            //await TestAccount();

            //Log("\n=== тестирование категорий ===");
            //await TestCategory();

            //Log("\n=== тестирование создание транзакций ===");
            //await TestTransaction();

            //Log("\n=== тестирование редактирования ===");
            //await UpdateTransaction();

            //Log("\n=== тестирование удаления ===");
            //await DeleteTest();

            //Log("\n=== тестирование аналитики ===");
            //await TestAnalytics();
        }

        #region создание счетов
        private async Task TestAccount()
        {
            Log("Cоздание счетов...");
            var account = new Account
            {
                Name = "Новая карта",
                Balance = 10000
            };
            await _accountProcessor.AddAccount(account);

            await OutputBalance();
        }
        #endregion

        #region создание категорий
        private async Task TestCategory()
        {
            Log("Создание категорий...");
            var category = new Category
            {
                Name = "транспорт",
                Icon = "🚕",
                Type = "expense"
            };
            await _categoryProcessor.AddCategory(category);

            //var category2 = new Category
            //{
            //    Name = "ЗП",
            //    Icon = "📂",
            //    Type = "income"
            //};
            //await _categoryProcessor.AddCategory(category2);

            Log("Категории:");
            var categories = await _categoryProcessor.GetAllCategory();
            foreach (var c in categories)
            {
                Log($"id({c.Id}) {c.Icon} {c.Name}, тип {c.Type}");
            }
        }
        #endregion

        #region создание транзакций
        private async Task TestTransaction()
        {
            //Log("\nСоздание транзакций...");
            var transaction = new Transaction
            {
                Type = "expense",
                Amount = 850,
                Account_id = 12,
                Category_id = 13,
                Comment = "Такси",
                Date = DateTime.Now
            };
            await _transactionProcessor.AddTransaction(transaction);

            var transaction2 = new Transaction
            {
                Type = "income",
                Amount = 9000,
                Account_id = 14,
                Category_id = 12,
                Comment = "зп",
                Date = DateTime.Now
            };
            await _transactionProcessor.AddTransaction(transaction2);

            await OutputTransaction();

            Log("\nCостояния счетов после создания транзакций:"); await OutputBalance();
        }
        #endregion

        #region аналитика
        private async Task TestAnalytics()
        {
            Log("Получение общего баланса...");
            var balance = await _analyticsService.GetTotalBalance();
            Log($"Общий баланс: {balance}");

            Log("\nПолучение прироста средств за месяц");
            var profit = await _analyticsService.GetProfit(DateTime.Now.AddMonths(-1), DateTime.Now);
            Log($"Прирост средств за месяц: {profit}");
        }
        #endregion

        #region редактирование транзакции
        private async Task UpdateTransaction()
        {
            Log("Получение счетов до редактирования:"); await OutputBalance();

            Log("\nРедактирование транзакции...");
            var transaction = new Transaction
            {
                Id = 48,
                Type = "expense",
                Amount = 2500,
                Account_id = 13,
                Category_id = 11,
                Comment = "Покупка еды редактированно",
                Date = DateTime.Now
            };
            await _transactionProcessor.UpdateTransaction(transaction);

            var transactionIncome = new Transaction
            {
                Id = 49,
                Type = "income",
                Amount = 500,
                Account_id = 12,
                Category_id = 12,
                Comment = "ЗП редактированно",
                Date = DateTime.Now
            };
            await _transactionProcessor.UpdateTransaction(transactionIncome);

            Log("Получение транзакций после редактирования:");
            await OutputTransaction();

            Log("Получение счетов после редактирования:"); await OutputBalance();
        }
        #endregion

        #region тест на удаление проверка FOREIGN KEY
        private async Task DeleteTest()
        {
            Log("Вывод до удаления");
            await OutputBalance();
            await OutputTransaction();

            await _categoryProcessor.DeleteCategory(13);

            Log("Вывод после удаления");
            await OutputBalance();
            await OutputTransaction();
        }
        #endregion

        private void Log(string message)
        {
            File.AppendAllText(LogFile, message + "\n");
        }

        private async Task OutputBalance()
        {
            Log("Текущие счета:");
            var accounts = await _accountProcessor.GetAllAccounts();
            foreach (var a in accounts)
            {
                Log($"Счет: {a.Name}, Баланс {a.Balance}");
            }
        }

        private async Task OutputTransaction()
        {
            Log("Текущие транзакций:");
            var transactions = await _transactionProcessor.GetAllTransaction();
            foreach (var t in transactions)
            {
                Log($"- Кат: {t.Category_id}. Счет: {t.Account_id}\n{t.Type} {t.Amount} руб. {t.Comment}");
            }
        }
    }
}
