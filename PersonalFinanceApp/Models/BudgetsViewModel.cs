namespace PersonalFinanceApp.Models
{
    public class BudgetsViewModel
    {
        public int Budgets_id { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }

        public int Category_id { get; set; }
        public double Amount { get; set; }
        public double Spent { get; set; }
        public double Remaining => Amount - Spent;
        public double Progress => Spent / Amount;
    }
}
