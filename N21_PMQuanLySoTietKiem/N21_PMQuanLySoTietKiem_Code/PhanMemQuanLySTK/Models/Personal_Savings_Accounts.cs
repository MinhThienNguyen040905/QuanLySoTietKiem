using PhanMemQuanLySTK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhanMemQuanLySTK.Models
{
    public class Personal_Savings_Accounts
    {
        public int Saving_ID { get; set; }
        public required string Name { get; set; }
        public required DateTime Creating_Date { get; set; }
        public required long Money { get; set; }
        public long? Target { get; set; }
        public required string Status { get; set; } // Đang hoạt động, Không hoạt động
        public required string Description { get; set; }

        public required string Username { get; set; }
        public virtual Users? User { get; set; }

        public required int Interest_Rate_ID { get; set; }
        public virtual Interest_Rates? Interest_Rate { get; set; }

        public virtual ICollection<Personal_Transactions_Information>? Personal_Transactions_Information
        { get; set; }

        public virtual ICollection<Personal_Notifications>? Personal_Notifications
        { get; set; }
    }
}
