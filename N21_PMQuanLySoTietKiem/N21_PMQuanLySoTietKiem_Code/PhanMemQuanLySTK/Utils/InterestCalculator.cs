using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhanMemQuanLySTK.Utils
{
    public class InterestCalculator
    {
        public static long CalculateInterestByDays(long principal, double annualRate, int termInDays)
        {
            double dailyRate = annualRate / 100 / 365;

            double interest = principal * dailyRate * termInDays;

            return (long)Math.Ceiling(interest); // làm tròn lên
        }
    }
}
