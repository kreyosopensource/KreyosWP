using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Kreyos.Classes.Utils
{
    public class KreyosUtils
    {
        /****************************************************************
         * Constants
         **/
        public const uint MIN_CHARS                     = 8;
        public const string EMAIL_DEFAULT               = " Email";
        public const string USERNAME_DEFAULT            = " Username";
        public const string PASSWORD_DEFAULT            = " Password";
        public const string CONFIRM_PASSWORD_DEFAULT    = " Confirm Password";
        public const string EMPTY_DEFAULT               = "";

        public static void ClearField (TextBox p_dummyField, int p_size, string p_default = "")
        {
            if (p_size <= 0)
            {
                p_dummyField.Text = p_default;
            }
            else
            {
                p_dummyField.Text = EMPTY_DEFAULT;
            }
        }

        /****************************************************************
         * Epoch/Time Utilities
         **/
        public static DateTime Now ()
        {
            return DateTime.Now;
        }
        
        public static DateTime NowWith (int p_hour, int p_min)
        {
            DateTime now = DateTime.Now;
            now.AddHours(p_hour);
            now.AddMinutes(p_min);
            return now;
        }

        public static DateTime ToDateTime (long p_epoch)
        {
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            epoch = epoch.AddSeconds(p_epoch);
            return epoch;
        }

        public static uint EpochFromNow ()
        {
            return EpochFrom(Now());
        }

        public static uint EpochFrom (DateTime p_time)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan diff = p_time.ToUniversalTime() - origin;
            return (uint)Math.Floor(diff.TotalSeconds);
        }

        public static string DateString (DateTime p_epochTime)
        {
            string day = p_epochTime.DayOfWeek.ToString();
            string date = p_epochTime.Month + " " + p_epochTime.Day;
            return day + " " + date;
        }

        public static string DateStringWithYear (DateTime p_epochTime)
        {
            string day = p_epochTime.DayOfWeek.ToString();
            string date = p_epochTime.Month + " " + p_epochTime.Day;
            string year = p_epochTime.Year.ToString();
            return day + " " + date + " " + year;
        }

        /****************************************************************
         * Date Utilities
         **/
        public static readonly string[] MONTHS = { "", "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dev" };
        public static string Month (int p_month)
        {
            return MONTHS[p_month];
        }

        /****************************************************************
         * Log Utilities
         **/
        public static void Log(string p_header, string p_message)
        {
            Debug.WriteLine("::::[KreyosLog] " + p_header + " " + p_message + "");
        }
    }
}
