using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhanMemQuanLySTK.Models
{
    public class Group_Notifications_Details
    {
        public required bool Is_Deleted { get; set; }

        public required string Status { get; set; } // Chưa đọc, Đã đọc, Chưa trả lời, Đồng ý, Không đồng ý

        public required int Group_Notification_ID { get; set; }
        public virtual Group_Notifications? Group_Notifications { get; set; }

        public required string Username { get; set; }
        public virtual Users? User { get; set; }

        [NotMapped]
        public bool Is_Selected { get; set; } = false;
        [NotMapped]
        public bool Is_Open { get; set; } = false;
    }
}
