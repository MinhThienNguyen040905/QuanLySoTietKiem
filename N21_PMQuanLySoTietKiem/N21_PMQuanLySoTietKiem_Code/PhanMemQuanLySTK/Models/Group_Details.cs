using PhanMemQuanLySTK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhanMemQuanLySTK.Models
{
    public class Group_Details
    {
        public required long Total_Money { get; set; } // Nạp cộng, rút trừ
        public required int Saving_ID { get; set; }
        public virtual Group_Savings_Accounts? Group_Savings_Accounts { get; set; }
        public required bool Is_Owner { get; set; }
        public required string Username { get; set; }
        public virtual Users? User { get; set; }
    }
}
