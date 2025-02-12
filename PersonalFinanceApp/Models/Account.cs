using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalFinanceApp.Models
{
    public class Account
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Balance { get; set; }
    }
}
