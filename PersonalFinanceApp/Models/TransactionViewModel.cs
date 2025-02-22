namespace PersonalFinanceApp.Models
{
    public class TransactionViewModel
    {
        public int Transaction_id { get; set; }
        public string Type { get; set; }
        public double Amount { get; set; }
        public string Comment { get; set; }
        public DateTime Date { get; set; }

        public string CategoryName { get; set; }
        public string CategoryIcon { get; set; }
        public string AccountName { get; set; }
        public string AmountColor { get; set; }
    }
}
