using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalFinanceApp.Data
{
    public class Account
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public float Balance { get; set; }
    }
}
