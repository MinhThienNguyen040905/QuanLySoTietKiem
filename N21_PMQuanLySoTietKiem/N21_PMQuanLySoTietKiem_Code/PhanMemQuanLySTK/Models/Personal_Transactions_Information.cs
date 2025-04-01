using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhanMemQuanLySTK.Models
{
    public class Personal_Transactions_Information
    {
        public int Transaction_ID { get; set; }
        public required DateTime Transaction_Date { get; set; }
        public required long Money { get; set; }
        public required string Description { get; set; }

        public required int Saving_ID { get; set; }

        public virtual Personal_Savings_Accounts? Personal_Savings_Accounts { get; set; }
    }
}
