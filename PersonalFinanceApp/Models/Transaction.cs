using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PersonalFinanceApp.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public double Amount { get; set; }
        public int AccountId { get; set; }
        public int CategoryId { get; set; }
        public string Comment { get; set; }
        public DateTime Date { get; set; }
    }
}
