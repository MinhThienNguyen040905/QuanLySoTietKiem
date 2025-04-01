using PhanMemQuanLySTK.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhanMemQuanLySTK.Models
{
    public class Personal_Notifications
    {
        public int Personal_Notification_ID { get; set; }
        public required long Money_Rate { get; set; }
        public required bool Is_Read { get; set; }
        public required DateTime Notification_Date { get; set; }

        public required bool Is_Deleted { get; set; }

        public required int Saving_ID { get; set; }
        public virtual Personal_Savings_Accounts? Personal_Savings_Account { get; set; }

        [NotMapped]
        public bool Is_Selected { get; set; } = false;
        [NotMapped]
        public bool Is_Open { get; set; } = false;
    }
}
