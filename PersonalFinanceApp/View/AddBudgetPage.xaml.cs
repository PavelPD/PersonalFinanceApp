using PersonalFinanceApp.BusinessLogic;
using PersonalFinanceApp.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace PersonalFinanceApp.View;

public partial class AddBudgetPage : ContentPage
{
    public static event EventHandler BudgetAdded;

	private readonly BudgetProcessor _budgetProcessor;
    private readonly CategoryProcessor _categoryProcessor;

    public string BudgetName { get; set; }
	public double Amount { get; set; }
	public ObservableCollection<Category> Categories { get; set; }
	public Category SelectedCategory { get; set; }

	public ICommand SaveCommand { get;}
	public ICommand CancelCommand { get; }

	public AddBudgetPage()
	{
		InitializeComponent();

		_budgetProcessor = App.BudgetProcessor;
        _categoryProcessor = App.CategoryProcessor;

		SaveCommand = new Command(async () => await SaveBudget());
		CancelCommand = new Command(async () => await Navigation.PopModalAsync());

		LoadData();
		BindingContext = this;
	}

	private async void LoadData()
	{
        var categories = (await _categoryProcessor.GetAllCategory()).Where(c => c.Type == "expense");
        Categories = new ObservableCollection<Category>(categories);

		OnPropertyChanged(nameof(Categories));
		OnPropertyChanged(nameof(SelectedCategory));
    }

	private async Task SaveBudget()
	{
		if(string.IsNullOrWhiteSpace(BudgetName) || SelectedCategory == null)
		{
			await DisplayAlert("Ошибка", "Не все поля заполнены", "Ок");
			return;
		}

		var budget = new Budget
		{
			Name = BudgetName,
			Category_id = SelectedCategory.Id,
			Amount = Amount
		};

		string result = await _budgetProcessor.AddBudget(budget);

        if (result != "OK")
        {
            await DisplayAlert("Ошибка", result, "ok");
            return;
        }

        BudgetAdded?.Invoke(this, EventArgs.Empty);
		await Navigation.PopModalAsync();
	}
}