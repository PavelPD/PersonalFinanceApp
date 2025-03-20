using PersonalFinanceApp.BusinessLogic;
using PersonalFinanceApp.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace PersonalFinanceApp.View;

public partial class NewTransactionPage : ContentPage
{
    public static event EventHandler TransactionAdded;

	private readonly TransactionProcessor _transactionProcessor;
	private readonly CategoryProcessor _categoryProcessor;
	private readonly AccountProcessor _accountProcessor;

	public ObservableCollection<Account> Accounts { get; set; } = new();
	public ObservableCollection<Category> Categories { get; set; } = new();

	public string SelectedTransactionType { get; set; } = "expense";
	public Account SelectedAccount { get; set; }
	public Category SelectedCategory { get; set; }
	public double Amount { get; set; }
	public string Comment { get; set; }
	public DateTime TransactionDate { get; set; } = DateTime.Now;

	public ICommand AddTransactionCommand { get; }
    public ICommand SelectedExpensesCommand { get; }
    public ICommand SelectedIncomeCommand { get; }

    public string ExpensesButtonColor { get; set; }
    public string IncomeButtonColor { get; set; }
    public string ExpensesTextColor { get; set; }
    public string IncomeTextColor { get; set; }

    private string MainBackground = "#272727";
    private string MainForeground = "#F3F3F3";

    public NewTransactionPage()
	{
		InitializeComponent();

		_transactionProcessor = App.TransactionProcessor;
		_categoryProcessor = App.CategoryProcessor;
		_accountProcessor = App.AccountProcessor;

		AddTransactionCommand = new Command(async () => await AddTransaction());

		SelectedExpensesCommand = new Command(() => SetTransactionType("expense"));
		SelectedIncomeCommand = new Command(() => SetTransactionType("income"));

        ExpensesButtonColor = MainForeground;
        ExpensesTextColor = MainBackground;
        IncomeButtonColor = MainBackground;
        IncomeTextColor = MainForeground;

        LoadData();
		BindingContext = this;

        AddAccountPage.AccountAdded += (s, e) => LoadData();
        EditAccountPage.AccountUpdated += (s, e) => LoadData();
        AddCategoryPage.CategoryAdded += (s, e) => LoadData();
        EditCategoryPage.CategoryUpdated += (s, e) => LoadData();

    }

    private async void LoadData()
	{		
		var accounts = await _accountProcessor.GetAllAccounts();
		var categories = await _categoryProcessor.GetAllCategory();

		Accounts = new ObservableCollection<Account>(accounts);
		Categories = new ObservableCollection<Category>(categories.Where(c => c.Type == SelectedTransactionType));

		OnPropertyChanged(nameof(Accounts));
		OnPropertyChanged(nameof(Categories));
	}

    private async Task AddTransaction()
	{
        if (SelectedCategory == null || SelectedAccount == null)
        {
            await DisplayAlert("Ошибка", "введите все данные", "Ок");
            return;
        }

        var newTransaction = new Transaction
        {
            Type = SelectedTransactionType,
            Amount = Amount,
            Account_id = SelectedAccount.Id,
            Category_id = SelectedCategory.Id,
            Comment = Comment,
            Date = TransactionDate
        };

        string result = await _transactionProcessor.AddTransaction(newTransaction);

        if (result != "OK")
        {
            await DisplayAlert("Ошибка", result, "Ок");
            return;
        }

        ClearPage();

        TransactionAdded?.Invoke(this, EventArgs.Empty);
        HideKeyboard.Hide();
    }

	private void SetTransactionType(string type)
	{
        SelectedTransactionType = type;

        ExpensesButtonColor = type == "expense" ? MainForeground : MainBackground;
        ExpensesTextColor = type == "expense" ? MainBackground : MainForeground;

        IncomeButtonColor = type == "income" ? MainForeground : MainBackground;
        IncomeTextColor = type == "income" ? MainBackground : MainForeground;

        SelectedCategory = null;

        OnPropertyChanged(nameof(ExpensesButtonColor));
        OnPropertyChanged(nameof(ExpensesTextColor));
        OnPropertyChanged(nameof(IncomeButtonColor));
        OnPropertyChanged(nameof(IncomeTextColor));

        OnPropertyChanged(nameof(SelectedCategory));

        LoadData();
    }   

    private void ClearPage()
    {
        Amount = 0;
        Comment = string.Empty;
        SelectedAccount = null;
        SelectedCategory = null;
        TransactionDate = DateTime.Now;

        OnPropertyChanged(nameof(Amount));
        OnPropertyChanged(nameof(Comment));
        OnPropertyChanged(nameof(SelectedAccount));
        OnPropertyChanged(nameof(SelectedCategory));
        OnPropertyChanged(nameof(TransactionDate));
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        ClearPage();
    }
}