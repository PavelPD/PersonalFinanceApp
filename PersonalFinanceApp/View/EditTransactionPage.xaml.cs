using PersonalFinanceApp.BusinessLogic;
using PersonalFinanceApp.Models;
using System.Windows.Input;

namespace PersonalFinanceApp.View;

public partial class EditTransactionPage : ContentPage
{
	public static event EventHandler TransactionUpdated;

	private readonly TransactionProcessor _transactionProcessor;
	private readonly CategoryProcessor _categoryProcessor;
	private readonly AccountProcessor _accountProcessor;

    public TransactionViewModel transactionViewModel { get; set; }
	public List<Category> Categories { get; set; }
	public List<Account> Accounts { get; set; }

	public Category SelectedCategory { get; set; }
	public Account SelectedAccount { get; set; }

	public ICommand SaveCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand CancelCommand { get; }

	public EditTransactionPage(TransactionViewModel originalTransaction)
	{
		InitializeComponent();

		_transactionProcessor = App.TransactionProcessor;
		_categoryProcessor = App.CategoryProcessor;
		_accountProcessor = App.AccountProcessor;

		transactionViewModel = new TransactionViewModel();
		transactionViewModel = originalTransaction;

		SaveCommand = new Command(async () => await SaveTransaction());
        DeleteCommand = new Command(async () => await DeleteTransaction());
        CancelCommand = new Command(async () => Navigation.PopModalAsync());

		LoadData();

		BindingContext = this;
	}

    private async void LoadData()
	{
        var allCategories = await _categoryProcessor.GetAllCategory();
		Categories = allCategories.Where(c => c.Type == transactionViewModel.Type).ToList();

        Accounts = await _accountProcessor.GetAllAccounts();

		SelectedCategory = Categories.FirstOrDefault(c => c.Name == transactionViewModel.CategoryName);
		SelectedAccount = Accounts.FirstOrDefault(a => a.Name == transactionViewModel.AccountName);

		OnPropertyChanged(nameof(Categories));
		OnPropertyChanged(nameof(Accounts));
        OnPropertyChanged(nameof(SelectedCategory));
        OnPropertyChanged(nameof(SelectedAccount));
    }   
	private async Task SaveTransaction()
	{
		var updatedTransaction = new Transaction
		{
			Id = transactionViewModel.Transaction_id,
			Type = transactionViewModel.Type,
			Amount = transactionViewModel.Amount,
			Comment = transactionViewModel.Comment,
			Date = transactionViewModel.Date,
			Category_id = SelectedCategory.Id,
			Account_id = SelectedAccount.Id,
		};

		string result = await _transactionProcessor.UpdateTransaction(updatedTransaction);
		
		if (result != "OK")
		{
			await DisplayAlert("Ошибка", result, "ok");
			return;
		} 

		TransactionUpdated?.Invoke(this, EventArgs.Empty);
		await Navigation.PopModalAsync();
	}

    private async Task DeleteTransaction()
    {
		await _transactionProcessor.DeleteTransaction(transactionViewModel.Transaction_id);

        TransactionUpdated?.Invoke(this, EventArgs.Empty); 
		await Navigation.PopModalAsync();
    }
}