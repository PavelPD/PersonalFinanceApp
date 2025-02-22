using PersonalFinanceApp.BusinessLogic;
using PersonalFinanceApp.Data;
using PersonalFinanceApp.Data.Repositories;
using PersonalFinanceApp.View;
using System.Diagnostics;

namespace PersonalFinanceApp
{
    public partial class App : Application
    {
        public static AnalyticsService AnalyticsService { get; private set; }
        public static TransactionProcessor TransactionProcessor { get; private set; }
        public static AccountProcessor AccountProcessor { get; private set; }
        public static CategoryProcessor CategoryProcessor { get; private set; }
        public static BudgetProcessor BudgetProcessor { get; private set; }


        public App()
        {
            InitializeComponent();

            var transactionRepository = new TransactionRepository();
            var accountRepository = new AccountRepository();
            var categoryRepository = new CategoryRepository();
            var budgetRepository = new BudgetRepository();

            AnalyticsService = new AnalyticsService(transactionRepository, accountRepository, categoryRepository);
            TransactionProcessor = new TransactionProcessor(transactionRepository, categoryRepository, accountRepository, budgetRepository);
            AccountProcessor = new AccountProcessor(accountRepository, transactionRepository, budgetRepository);
            CategoryProcessor = new CategoryProcessor(categoryRepository, transactionRepository, accountRepository);
            BudgetProcessor = new BudgetProcessor(budgetRepository, categoryRepository, transactionRepository);            

            MainPage = new AppShell();
        }

        protected override async void OnStart()
        {
            base.OnStart();
            await DBInitializer.InitializeDatabase();

            //класс тестов
            //var testConsole = new TestConsole();
            //await testConsole.RunTest();
        }
    }
}
