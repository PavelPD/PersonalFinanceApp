using PersonalFinanceApp.BusinessLogic;
using PersonalFinanceApp.Data;
using PersonalFinanceApp.Data.Repositories;
using PersonalFinanceApp.View;
using System.Diagnostics;

namespace PersonalFinanceApp
{
    public partial class App : Application
    {
        public static AnalyticsService analyticsService { get; private set; }

        public App()
        {
            InitializeComponent();

            var transactionRepository = new TransactionRepository();
            var accountRepository = new AccountRepository();
            var categoryRepository = new CategoryRepository();

            analyticsService = new AnalyticsService(transactionRepository, accountRepository, categoryRepository);
            MainPage = new HomePage { BindingContext = analyticsService };
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
