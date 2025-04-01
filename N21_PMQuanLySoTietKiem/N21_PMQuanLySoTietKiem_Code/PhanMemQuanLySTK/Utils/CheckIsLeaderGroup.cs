using PhanMemQuanLySTK.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhanMemQuanLySTK.Utils
{
    public class CheckIsLeaderGroup
    {
        public static bool IsLeaderGroup(string username, int saving_Id)
        {
            using (var context = new AppDbContext())
            {
                var leader = context.Group_Details
                             .FirstOrDefault(d => d.Saving_ID == saving_Id && d.Username == username && d.Is_Owner == true);

                return leader != null;
            }
        }
    }
}
