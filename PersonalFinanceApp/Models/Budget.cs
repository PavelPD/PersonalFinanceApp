namespace PersonalFinanceApp.Models
{
    public class Budget
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Category_id { get; set; }
        public double Amount { get; set; }
    }
}
