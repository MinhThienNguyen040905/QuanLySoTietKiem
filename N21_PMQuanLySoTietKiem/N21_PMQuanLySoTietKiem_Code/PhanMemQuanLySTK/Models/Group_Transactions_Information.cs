using PhanMemQuanLySTK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhanMemQuanLySTK.Models
{
    public class Group_Transactions_Information
    {
        public int Transaction_ID { get; set; }
        public required DateTime Transaction_Date { get; set; }
        public required long Money { get; set; }
        public required string Description { get; set; }

        public required int Saving_ID { get; set; }
        public virtual Group_Savings_Accounts? Group_Savings_Accounts { get; set; }

        public string? Username { get; set; }
        public virtual Users? User { get; set; }
    }
}
