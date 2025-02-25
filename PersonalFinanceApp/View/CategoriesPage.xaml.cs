using PersonalFinanceApp.BusinessLogic;
using PersonalFinanceApp.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace PersonalFinanceApp.View;

public partial class CategoriesPage : ContentPage
{
    public ObservableCollection<Category> ExpensesCategories { get; set; }
    public ObservableCollection<Category> IncomeCategories { get; set; }

    private readonly CategoryProcessor _categoryProcessor;

    public ICommand AddCategoryCommand { get; }
    public ICommand EditCategoryCommand { get; }
    public ICommand CancelCommand { get; }

    public CategoriesPage()
	{
		InitializeComponent();

        _categoryProcessor = App.CategoryProcessor;

        ExpensesCategories = new ObservableCollection<Category>();
        IncomeCategories = new ObservableCollection<Category>();


        AddCategoryCommand = new Command(async () => await OpenAddCategoryPage());
        EditCategoryCommand = new Command<Category>(async (category) => await OpenEditCategoryPage(category));
        CancelCommand = new Command(async () => await Navigation.PopModalAsync());

        BindingContext = this;
        LoadCategories();

        AddCategoryPage.CategoryAdded += (s, e) => LoadCategories();
        EditCategoryPage.CategoryUpdated += (s, e) => LoadCategories();
    }

    private async void LoadCategories()
    {
        var expensesCategories = (await _categoryProcessor.GetAllCategory()).Where(c => c.Type == "expense");
        ExpensesCategories.Clear();
        foreach (var category in expensesCategories)
        {
            ExpensesCategories.Add(category);
        }

        var incomeCategories = (await _categoryProcessor.GetAllCategory()).Where(c => c.Type == "income");
        IncomeCategories.Clear();
        foreach (var category in incomeCategories)
        {
            IncomeCategories.Add(category);
        }
    }

    private async Task OpenAddCategoryPage()
    {
        await Navigation.PushModalAsync(new AddCategoryPage());
    }

    private async Task OpenEditCategoryPage(Category category)
    {
        await Navigation.PushModalAsync(new EditCategoryPage(category));
    }
}
