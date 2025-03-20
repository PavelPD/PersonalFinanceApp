using PersonalFinanceApp.BusinessLogic;
using PersonalFinanceApp.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace PersonalFinanceApp.View;

public partial class EditCategoryPage : ContentPage
{
    public static event EventHandler CategoryUpdated;

    private readonly CategoryProcessor _categoryProcessor;
    
    public ObservableCollection<string> EmojiList {  get; set; }
    public Category Category { get; set; }

    public ICommand SaveCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand CancelCommand { get; }

    public EditCategoryPage(Category category)
	{
		InitializeComponent();
        
        LoadEmojiList();

        _categoryProcessor = App.CategoryProcessor;

        Category = new Category
        {
            Id = category.Id,
            Name = category.Name,
            Icon = category.Icon,
            Type = category.Type
        };

        SaveCommand = new Command(async () => await SaveCategory());
        DeleteCommand = new Command(async () => await DeleteCategory());
        CancelCommand = new Command(async () => {
            await Navigation.PopModalAsync();
            HideKeyboard.Hide();
        });

        BindingContext = this;
    }

    private async Task SaveCategory()
    {
        string result = await _categoryProcessor.UpdateCategory(Category);
        
        if (result != "OK")
        {
            await DisplayAlert("Ошибка", result, "ok");
            return;
        }

        CategoryUpdated?.Invoke(this, EventArgs.Empty);

        HideKeyboard.Hide();
        await Navigation.PopModalAsync();
    }

    private async Task DeleteCategory()
    {
        bool confirm = await DisplayAlert("Удаление", "Вы уверены, что хотите удалить категорию? Вместе с ней удалятся все связанные транзакции", "Да", "Нет");
        if (!confirm) return;

        await _categoryProcessor.DeleteCategory(Category.Id);
        CategoryUpdated?.Invoke(this, EventArgs.Empty);

        HideKeyboard.Hide();
        await Navigation.PopModalAsync();
    }

    private async void LoadEmojiList()
    {
        EmojiList = new ObservableCollection<string>
        {
            "🍔", "🚗", "🏠", "📱", "💊",
            "🍎", "🚌", "🔍", "👕", "🏥",
            "🛒", "🎁", "🎬", "🎮", "💻",
            "🍽", "✈️", "🐾", "🏦", "♻️",
            "🍹", "🏸", "📚", "💼", "🎂",
            "🔥", "🏃", "🎓", "💵", "📦"
        };

    }

    private void OnEmojiSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.Count > 0)
        {
            Category.Icon = e.CurrentSelection[0] as string;
            OnPropertyChanged(nameof(Category));
        }
    }
}