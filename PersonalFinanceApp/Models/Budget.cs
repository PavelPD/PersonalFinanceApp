using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalFinanceApp.Models
{
    public class Budget
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Category_id { get; set; }
        public double Amount { get; set; }
        public double Spent { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
    }
}
