using PersonalFinanceApp.BusinessLogic;
using PersonalFinanceApp.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace PersonalFinanceApp.View;

public partial class BudgetsPage : ContentPage
{
    public ObservableCollection<BudgetsViewModel> BudgetsList { get; set; }

    private readonly BudgetProcessor _budgetProcessor;

    public ICommand AddBudgetCommand { get; }
    public ICommand EditBudgetCommand { get; }

    public BudgetsPage()
    {
        InitializeComponent();

        _budgetProcessor = App.BudgetProcessor;

        BudgetsList = new ObservableCollection<BudgetsViewModel>();

        AddBudgetCommand = new Command(async () => await OpenAddBudgetPage());
        EditBudgetCommand = new Command<BudgetsViewModel>(async (budget) => await OpenEditBudgetPage(budget));

        BindingContext = this;
        LoadBudgets();

        EditBudgetPage.BudgetUpdated += (s, e) => LoadBudgets();
        AddBudgetPage.BudgetAdded += (s, e) => LoadBudgets();
        EditTransactionPage.TransactionUpdated += (s, e) => LoadBudgets();
        NewTransactionPage.TransactionAdded += (s, e) => LoadBudgets();
        EditAccountPage.AccountUpdated += (s, e) => LoadBudgets();
        EditCategoryPage.CategoryUpdated += (s, e) => LoadBudgets();
        SettingsPage.DbRest += (s, e) => LoadBudgets();
    }

    private async void LoadBudgets()
    {
        var budgets = await _budgetProcessor.GetAllBudgetWithSpent();
        BudgetsList.Clear();

        foreach (var budget in budgets)
        {
            BudgetsList.Add(budget);
        }
    }

    private async Task OpenAddBudgetPage()
    {
        await Navigation.PushModalAsync(new AddBudgetPage());
    }

    private async Task OpenEditBudgetPage(BudgetsViewModel budget)
    {
        await Navigation.PushModalAsync(new EditBudgetPage(budget));
    }
}