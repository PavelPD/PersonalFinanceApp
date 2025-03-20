using PersonalFinanceApp.BusinessLogic;
using PersonalFinanceApp.Models;
using System.Windows.Input;

namespace PersonalFinanceApp.View;

public partial class EditAccountPage : ContentPage
{
    public static event EventHandler AccountUpdated;

    private readonly AccountProcessor _accountProcessor;

    public Account Account { get; set; }

    public ICommand SaveCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand CancelCommand { get; }

    public EditAccountPage(Account account)
    {
        InitializeComponent();

        _accountProcessor = App.AccountProcessor;

        Account = new Account
        {
            Id = account.Id,
            Name = account.Name,
            Balance = account.Balance
        };

        SaveCommand = new Command(async () => await SaveAccount());
        DeleteCommand = new Command(async () => await DeleteAccount());
        CancelCommand = new Command(async () => {
            await Navigation.PopModalAsync();
            HideKeyboard.Hide();
        });

        BindingContext = this;
    }

    private async Task SaveAccount()
    {
        string result = await _accountProcessor.UpdateAccount(Account);

        if (result != "OK")
        {
            await DisplayAlert("Ошибка", result, "ok");
            return;
        }

        AccountUpdated?.Invoke(this, EventArgs.Empty);

        HideKeyboard.Hide();
        await Navigation.PopModalAsync();
    }

    private async Task DeleteAccount()
    {
        bool confirm = await DisplayAlert("Удаление", "Вы уверены, что хотите удалить счет? Вместе с ним удалятся все связанные транзакции", "Да", "Нет");
        if (!confirm) return;
        
        await _accountProcessor.DeleteAccount(Account.Id);

        AccountUpdated?.Invoke(this, EventArgs.Empty);

        HideKeyboard.Hide();
        await Navigation.PopModalAsync();
    }    
}