using System.Windows.Input;

namespace PersonalFinanceApp.View;

public partial class SettingsPage : ContentPage
{
    public static event EventHandler DbRest;

    public ICommand OpenAccountsCommand { get; }
    public ICommand OpenCategoriesCommand { get; }
    public ICommand ResetDbCommand { get; }

    public SettingsPage()
	{
		InitializeComponent();

		OpenAccountsCommand = new Command(async () => await OpenAccountPage());
        OpenCategoriesCommand = new Command(async () => await OpenCategoriesPage());
        ResetDbCommand = new Command(async () => await ResetDb());

        BindingContext = this;
	}

	private async Task OpenAccountPage()
	{
		await Navigation.PushModalAsync(new AccountsPage());
	}

    private async Task OpenCategoriesPage()
    {
        await Navigation.PushModalAsync(new CategoriesPage());
    }

    private async Task ResetDb()
    {
        bool confirm = await DisplayAlert("Подтверждение", "Вы уверены, что хотите удалить все данные?", "Удалить", "Отмена");

        if (!confirm) return;
        
        await App.DataBaseProcessor.ResetDatabase();
        DbRest?.Invoke(this, EventArgs.Empty);
    }
}