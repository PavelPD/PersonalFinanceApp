using PersonalFinanceApp.BusinessLogic;
using PersonalFinanceApp.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace PersonalFinanceApp.View;

public partial class AccountsPage : ContentPage
{
	public ObservableCollection<Account> Accounts { get; set; }

	private readonly AccountProcessor _accountProcessor;

	public ICommand AddAccountCommand { get; }
	public ICommand EditAccountCommand { get; }
    public ICommand CancelCommand { get; }


    public AccountsPage()
	{
		InitializeComponent();

		_accountProcessor = App.AccountProcessor;

		Accounts = new ObservableCollection<Account>();

		AddAccountCommand = new Command(async () => await OpenAddAccountPage());
		EditAccountCommand = new Command<Account>(async (account) => await OpenEditAccountPage(account));
        CancelCommand = new Command(async () => await Navigation.PopModalAsync());

        BindingContext = this;
		LoadAccounts();

		AddAccountPage.AccountAdded += (s, e) => LoadAccounts();
        EditAccountPage.AccountUpdated += (s, e) => LoadAccounts();
    }

    private async void LoadAccounts()
	{
		var accounts = await _accountProcessor.GetAllAccounts();
		Accounts.Clear();
		foreach (var account in accounts)
		{
			Accounts.Add(account);
		}
    }

	private async Task OpenAddAccountPage()
	{
		await Navigation.PushModalAsync(new AddAccountPage());
	}

    private async Task OpenEditAccountPage(Account account)
    {
        await Navigation.PushModalAsync(new EditAccountPage(account));
    }
}