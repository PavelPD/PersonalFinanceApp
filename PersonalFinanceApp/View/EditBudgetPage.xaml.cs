using PersonalFinanceApp.BusinessLogic;
using PersonalFinanceApp.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace PersonalFinanceApp.View;

public partial class EditBudgetPage : ContentPage
{
    public static event EventHandler BudgetUpdated;

	private readonly BudgetProcessor _budgetProcessor;
    private readonly CategoryProcessor _categoryProcessor;

    public BudgetsViewModel Budget { get; set; }
	public ObservableCollection<Category> Categories { get; set; }
    public Category SelectedCategory { get; set; }


    public ICommand SaveCommand { get; set; }
    public ICommand DeleteCommad { get; set; }
    public ICommand CancelCommand { get; }


    public EditBudgetPage(BudgetsViewModel budget)
	{
		InitializeComponent();
		_budgetProcessor = App.BudgetProcessor;
		_categoryProcessor = App.CategoryProcessor;

		Budget = budget;

		SaveCommand = new Command(async () => await SaveBudget());
		DeleteCommad = new Command(async () => await DeleteBudget());
        CancelCommand = new Command(async () => await Navigation.PopModalAsync());

        BindingContext = this;
		LoadData();
	}

	private async void LoadData()
	{
		var catedories = (await _categoryProcessor.GetAllCategory()).Where(c => c.Type == "expense");
        Categories = new ObservableCollection<Category>(catedories);

		SelectedCategory = Categories.FirstOrDefault(c => c.Id == Budget.Category_id);

        OnPropertyChanged(nameof(Categories));
        OnPropertyChanged(nameof(SelectedCategory));
    }

    private async Task SaveBudget()
	{
		if(string.IsNullOrWhiteSpace(Budget.Name))
		{
			await DisplayAlert("Ошибка", "Заполните все поля", "Ок");
			return;
		}

		var updatedBudget = new Budget
		{
			Id = Budget.Budgets_id,
			Name = Budget.Name,
			Category_id = SelectedCategory.Id,
			Amount = Budget.Amount
		};

        string result = await _budgetProcessor.UpdateBudget(updatedBudget);

        if (result != "OK")
        {
            await DisplayAlert("Ошибка", result, "ok");
            return;
        }

        BudgetUpdated?.Invoke(this, EventArgs.Empty);
        await Navigation.PopModalAsync();
    }

    private async Task DeleteBudget()
    {
		bool confirm = await DisplayAlert("Удаление", "Вы действительно хотите удалить бюджет?", "Да", "Нет");
		if (!confirm) return;

		await _budgetProcessor.DeleteBudget(Budget.Budgets_id);

		BudgetUpdated?.Invoke(this, EventArgs.Empty);
		await Navigation.PopModalAsync();
    }
}