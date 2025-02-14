using PersonalFinanceApp.Data;
using System.Diagnostics;

namespace PersonalFinanceApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new AppShell();
        }

        protected override async void OnStart()
        {
            base.OnStart();
            await DBInitializer.InitializeDatabase();

            var testConsole = new TestConsole();
            await testConsole.RunTest();
        }
    }
}
