using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceVision
{
    public class Helper
    {
        public static DateTime StartOfWeek
        {
            get
            {
                DateTime today = DateTime.Today;
                while (today.DayOfWeek != DayOfWeek.Sunday)
                    today = today.AddDays(-1);
                return today;
            }
        }

        public static DateTime EndOfWeek
        {
            get
            {
                DateTime today = DateTime.Today;
                while (today.DayOfWeek != DayOfWeek.Saturday)
                    today = today.AddDays(1);
                return today;
            }
        }

        public static bool IsThisWeek(DateTime date)
        {
            return DateTime.Compare(date, StartOfWeek) > 0 && DateTime.Compare(date, EndOfWeek) < 0 ? true : false;
        }
    }
}
