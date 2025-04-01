using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhanMemQuanLySTK.Models
{
    public class Interest_Rates
    {
        public int Interest_Rate_ID { get; set; }
        public required int Term { get; set; }
        public required double Rate { get; set; }

        public virtual ICollection<Personal_Savings_Accounts>? Personal_Savings_Accounts { get; set; }

        public virtual ICollection<Group_Savings_Accounts>? Group_Savings_Accounts { get; set; }
    }
}
