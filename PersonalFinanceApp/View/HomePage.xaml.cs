using PersonalFinanceApp.BusinessLogic;
using PersonalFinanceApp.Models;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.Windows.Input;

namespace PersonalFinanceApp.View;

public partial class HomePage : ContentPage
{
    public ObservableCollection<CategoryExpense> CategoryExpenses { get; set; }
    
    private AnalyticsService _analyticsService;

    private DateTime _currentDate = DateTime.Now;

    public ICommand PreviousMonthCommand { get; }
    public ICommand NextMonthCommand { get; }
    public ICommand ChartSwitchCommand { get; }
    public ICommand ShowExpensesCommand { get; }
    public ICommand ShowIncomeCommand { get; }
    
    public double Balance { get; set; }
    public double PeriodGrowth { get; set; }
    public string SelectedMonth { get; set; }
    public double TransactionAmountInMonth { get; set; }
    public string TransactionsForThisMonth {  get; set; }
    public double AveragePerDay { get; set; }
    public bool IsIncomeSelected { get; set; } = false;    

    public string ExpensesButtonColor { get; set; }
    public string IncomeButtonColor { get; set; }
    public string ExpensesTextColor { get; set; }
    public string IncomeTextColor { get; set; }

    private string MainBackground = "#272727";
    private string MainForeground = "#F3F3F3";

    public HomePage()
	{
        InitializeComponent();

        CategoryExpenses = new ObservableCollection<CategoryExpense>();

        PreviousMonthCommand = new Command(() => ChangeMonth(-1));
        NextMonthCommand = new Command(() => ChangeMonth(1));

        ChartSwitchCommand = new Command(() => ToggleExpensesIncome(!IsIncomeSelected));

        ShowExpensesCommand = new Command(() => ToggleExpensesIncome(false));
        ShowIncomeCommand = new Command(() => ToggleExpensesIncome(true));

        ExpensesButtonColor = MainForeground;
        ExpensesTextColor = MainBackground;
        IncomeButtonColor = MainBackground;
        IncomeTextColor = MainForeground;

        _analyticsService = App.AnalyticsService;
        BindingContext = this;
        LoadData();

        EditTransactionPage.TransactionUpdated += (sender, ards) => LoadData();
    }

    private async void LoadData()
    {
        if (_analyticsService == null) return;
        
        DateTime startMonth = new DateTime(_currentDate.Year, _currentDate.Month, 1);
        DateTime endMonth = startMonth.AddMonths(1).AddDays(-1);
        SelectedMonth = _currentDate.ToString("MMMM yyyy");
        
        //блок баланса и прироста
        Balance = await _analyticsService.GetTotalBalance();
        PeriodGrowth = Math.Round(await _analyticsService.GetProfit(startMonth, endMonth), 2);

        //блок транзакций за месяц и средний день
        TransactionsForThisMonth = (IsIncomeSelected ? "Доходы" : "Расходы") + $" за {_currentDate.ToString("MMM")}" ;
        TransactionAmountInMonth = await _analyticsService.GetTransactionsAmountForMonth(_currentDate, IsIncomeSelected);
                   
        var dayPassed = (_currentDate.Month == DateTime.Now.Month && _currentDate.Year == DateTime.Now.Year) 
            ? _currentDate.Day 
            : DateTime.DaysInMonth(_currentDate.Year, _currentDate.Month);
            
        AveragePerDay = TransactionAmountInMonth / dayPassed;
        

        //запрос списка трат по категориям
        var expensesList = await _analyticsService.GetCategoryExpenses(startMonth, endMonth, IsIncomeSelected);

        CategoryExpenses.Clear();
        foreach (var expense in expensesList)
        {
            CategoryExpenses.Add(expense);
        }

        OnPropertyChanged(nameof(Balance));
        OnPropertyChanged(nameof(PeriodGrowth));
        OnPropertyChanged(nameof(TransactionsForThisMonth));
        OnPropertyChanged(nameof(TransactionAmountInMonth));
        OnPropertyChanged(nameof(AveragePerDay));
        OnPropertyChanged(nameof(SelectedMonth));
    }

    private void ChangeMonth(int offset)
    {
        _currentDate = _currentDate.AddMonths(offset);
        LoadData();
    }   

    private void ToggleExpensesIncome(bool showExpenses)
    {
        IsIncomeSelected = showExpenses;
        UpdateButtonColors();
        LoadData();       
    }

    private void UpdateButtonColors()
    {
        ExpensesButtonColor = IsIncomeSelected ? MainBackground : MainForeground;
        ExpensesTextColor = IsIncomeSelected ? MainForeground : MainBackground;

        IncomeButtonColor = IsIncomeSelected ? MainForeground : MainBackground;
        IncomeTextColor = IsIncomeSelected ? MainBackground : MainForeground;

        OnPropertyChanged(nameof(ExpensesButtonColor));
        OnPropertyChanged(nameof(ExpensesTextColor));
        OnPropertyChanged(nameof(IncomeButtonColor));
        OnPropertyChanged(nameof(IncomeTextColor));
    }
}
