using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Hesabdar.Utility
{
    public class General
    {
        public static string EncodePasswordMd5(string pass)   
        {
            Byte[] originalBytes;
            Byte[] encodedBytes;
            MD5 md5;
            md5 = new MD5CryptoServiceProvider();
            originalBytes = ASCIIEncoding.Default.GetBytes(pass);
            encodedBytes = md5.ComputeHash(originalBytes);
            //Convert encoded bytes back to a 'readable' string    
            return BitConverter.ToString(encodedBytes);
        }
        public static string GetPersianMonthName(DateTime dateTime)
        {
            PersianCalendar persianCalendar = new PersianCalendar();
            int month = persianCalendar.GetMonth(dateTime);

            string[] persianMonthNames = {
            "فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور",
            "مهر", "آبان", "آذر", "دی", "بهمن", "اسفند"
        };

            return persianMonthNames[month - 1];
        }
        public static string ConvertToPersianDate(DateTime inDateTime) //Convert Orginal DateTime to Persian DateTime
        {
            PersianCalendar pc = new PersianCalendar();
            string persianDateTime = (string.Format("{0}/{1}/{2}"
                , pc.GetYear(inDateTime)
                , pc.GetMonth(inDateTime)
                , pc.GetDayOfMonth(inDateTime)
               ));
            return persianDateTime;
        }
    }
}
