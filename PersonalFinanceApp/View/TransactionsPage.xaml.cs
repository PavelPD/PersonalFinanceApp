using PersonalFinanceApp.BusinessLogic;
using PersonalFinanceApp.Models;
using System.Collections.ObjectModel;
using System.Runtime.ConstrainedExecution;
using System.Windows.Input;


namespace PersonalFinanceApp.View;

public partial class TransactionsPage : ContentPage
{
    private readonly TransactionProcessor _transactionProcessor;
    public ObservableCollection<TransactionViewModel> Transactions { get; set; }

    private DateTime _currentDate = DateTime.Now;
    public string SelectedMonth { get; set; }

    public ICommand PreviousMonthCommand { get; }
    public ICommand NextMonthCommand { get; }
    public Command<TransactionViewModel> EditCommand { get; }

    public TransactionsPage()
	{
		InitializeComponent();

        _transactionProcessor = App.TransactionProcessor;
        Transactions = new ObservableCollection<TransactionViewModel>();

        PreviousMonthCommand = new Command(() => ChangeMonth(-1));
        NextMonthCommand = new Command(() => ChangeMonth(1));
        EditCommand = new Command<TransactionViewModel>(async (transaction) => await OpenEditPage(transaction));

        BindingContext = this;
        LoadTransactions();

        EditTransactionPage.TransactionUpdated += (sender, ards) => LoadTransactions();
    }   
    
    private async void LoadTransactions()
    {
        SelectedMonth = _currentDate.ToString("MMMM yyyy");

        var transactionsList = await _transactionProcessor.GetAllTransactionViewModel(_currentDate);
        Transactions.Clear();

        foreach (var transaction in transactionsList)
        {
            Transactions.Add(transaction);
        }

        OnPropertyChanged(nameof(SelectedMonth));
    }

    private async void ChangeMonth(int offset)
    {
        _currentDate = _currentDate.AddMonths(offset);
        LoadTransactions();
    }

    private async Task OpenEditPage(TransactionViewModel transaction)
    {
        await Navigation.PushModalAsync(new EditTransactionPage(transaction));
    }
}