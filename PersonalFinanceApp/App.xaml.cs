﻿using PersonalFinanceApp.BusinessLogic;
using PersonalFinanceApp.Data;
using PersonalFinanceApp.Data.Repositories;
using PersonalFinanceApp.View;

namespace PersonalFinanceApp
{
    public partial class App : Application
    {
        public static AnalyticsService AnalyticsService { get; private set; }
        public static TransactionProcessor TransactionProcessor { get; private set; }
        public static AccountProcessor AccountProcessor { get; private set; }
        public static CategoryProcessor CategoryProcessor { get; private set; }
        public static BudgetProcessor BudgetProcessor { get; private set; }
        public static DataBaseProcessor DataBaseProcessor { get; private set; }

        public App()
        {
            InitializeComponent();

            var transactionRepository = new TransactionRepository();
            var accountRepository = new AccountRepository();
            var categoryRepository = new CategoryRepository();
            var budgetRepository = new BudgetRepository();
            var databaseRepository = new DataBaseRepository();

            AnalyticsService = new AnalyticsService(transactionRepository, accountRepository, categoryRepository);
            TransactionProcessor = new TransactionProcessor(transactionRepository, categoryRepository, accountRepository);
            AccountProcessor = new AccountProcessor(accountRepository);
            CategoryProcessor = new CategoryProcessor(categoryRepository, transactionRepository, accountRepository);
            BudgetProcessor = new BudgetProcessor(budgetRepository, categoryRepository, transactionRepository);
            DataBaseProcessor = new DataBaseProcessor(databaseRepository);

            MainPage = new LoadingPage();
            InitializeApp();
        }

        protected async void InitializeApp()
        {
            await DBInitializer.InitializeDatabase();

            MainPage = new AppShell();
        }
    }
}
