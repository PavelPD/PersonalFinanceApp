namespace PersonalFinanceApp.Models
{
    public class CategoryExpenseViewModel
    {
        public int Category_id { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public double Amount { get; set; }
        public double Percentage { get; set; }
    }
}
