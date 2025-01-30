using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalFinanceApp.Data
{
    public class Budget
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CategoryId { get; set; }
        public float Amount { get; set; }
        public float Spent { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
    }
}
