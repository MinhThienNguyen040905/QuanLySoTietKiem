using PhanMemQuanLySTK.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhanMemQuanLySTK.Models
{
    public class Group_Notifications
    {
        public int Group_Notification_ID { get; set; }
        public required string Description { get; set; }
        public required string Type { get; set; }// Nạp, Rút, RútYC, RútPH, MờiPH, MờiYC, Lãi, Xóa
        public long? Money { get; set; }// số tiền 

        public string? Username_Sender { get; set; }
        public virtual Users? User_Sender { set; get; }

        public required DateTime Notification_Date { get; set; }
        public virtual ICollection<Group_Notifications_Details>? Group_Notifications_Details
        { get; set; }

        public required int Saving_ID { get; set; }
        public virtual Group_Savings_Accounts? Group_Savings_Accounts { get; set; }
    }
}
