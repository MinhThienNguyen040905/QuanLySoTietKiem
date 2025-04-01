using PhanMemQuanLySTK.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhanMemQuanLySTK.Models
{
    public class Users
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required string Fullname { get; set; }
        public required string Email { get; set; }
        public required long Money { get; set; }
        public required string Gender { get; set; }
        public required DateTime Dob { get; set; }
        public required string Address { get; set; }
        public required string Phone { get; set; }
        public required string Avatar { get; set; }
        public required string Identity_Card { get; set; }

        public virtual ICollection<Group_Notifications>? Group_Notifications { get; set; }
        public virtual ICollection<Personal_Savings_Accounts>? Personal_Savings_Accounts { get; set; }
        public virtual ICollection<Group_Transactions_Information>? Group_Transactions_Information
        { get; set; }
        public virtual ICollection<Group_Details>? Group_Details { get; set; }
        public virtual ICollection<Group_Notifications_Details>? Group_Notifications_Details { get; set; }
    }
}
