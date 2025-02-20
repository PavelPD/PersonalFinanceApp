namespace PersonalFinanceApp.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public double Amount { get; set; }
        public int Account_id { get; set; }
        public int Category_id { get; set; }
        public string Comment { get; set; }
        public DateTime Date { get; set; }
    }
}
