using PersonalFinanceApp.BusinessLogic;
using PersonalFinanceApp.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace PersonalFinanceApp.View;

public partial class AddCategoryPage : ContentPage
{
    public static event EventHandler CategoryAdded;

    private readonly CategoryProcessor _categoryProcessor;
    public ObservableCollection<string> EmojiList { get; set; }

    public string CategoryName { get; set; }
    public string Icon { get; set; }
    public List<string> CategoryTypes { get; set; }
    public string SelectedType { get; set; }

    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }


    public AddCategoryPage()
    {
        InitializeComponent();
        
        LoadEmojiList();

        _categoryProcessor = App.CategoryProcessor;

        CategoryTypes = new List<string> { "Доход", "Расход" };

        SaveCommand = new Command(async () => await SaveCategory());
        CancelCommand = new Command(async () => await Navigation.PopModalAsync());

        BindingContext = this;
    }

    private async Task SaveCategory()
    {
        if (string.IsNullOrWhiteSpace(CategoryName) || string.IsNullOrWhiteSpace(SelectedType))
        {
            await DisplayAlert("Ошибка", "Заполните все поля", "Ок");
            return;
        }

        var newCategory = new Category
        {
            Name = CategoryName,
            Type = SelectedType == "Доход" ? "income" : "expense",
            Icon = Icon ?? "🗂"
        };

        string result = await _categoryProcessor.AddCategory(newCategory);

        if (result != "OK")
        {
            await DisplayAlert("Ошибка", result, "ok");
            return;
        }

        CategoryAdded?.Invoke(this, EventArgs.Empty);
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
            Icon = e.CurrentSelection[0] as string;
            OnPropertyChanged(nameof(Icon));

            var collectionView = sender as CollectionView;
            if(collectionView != null)
            {
                collectionView.SelectedItem = null;
            }
        }
    }
}