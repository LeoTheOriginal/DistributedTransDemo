using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransDemo.Models
{
    public class BranchTxnShare
    {
        public string BranchName { get; set; } = null!;
        public decimal SharePercent { get; set; }
    }
}
