using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransDemo.Models
{
    public class DailyTransactionStat
    {
        public DateTime TxnDate { get; set; }
        public int TxnCount { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
