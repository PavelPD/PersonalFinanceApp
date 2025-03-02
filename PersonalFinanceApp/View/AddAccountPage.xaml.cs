using PersonalFinanceApp.BusinessLogic;
using PersonalFinanceApp.Models;
using System.Windows.Input;

namespace PersonalFinanceApp.View;

public partial class AddAccountPage : ContentPage
{
    public static event EventHandler AccountAdded;

	private readonly AccountProcessor  _accountProcessor;

	public string AccountName { get; set; }
	public double Balance { get; set; }

	public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }


    public AddAccountPage()
	{
		InitializeComponent();

		_accountProcessor = App.AccountProcessor;

		SaveCommand = new Command(async () => await SaveAccount());
        CancelCommand = new Command(async () => await Navigation.PopModalAsync());

		BindingContext = this;
    }

	private async Task SaveAccount()
	{
		var newAccount = new Account
		{
			Name = AccountName,
			Balance = Balance
		};

		string result = await _accountProcessor.AddAccount(newAccount);

        if (result != "OK")
        {
            await DisplayAlert("Ошибка", result, "ok");
            return;
        }
		
		AccountAdded?.Invoke(this, EventArgs.Empty);
		await Navigation.PopModalAsync();
    }
}