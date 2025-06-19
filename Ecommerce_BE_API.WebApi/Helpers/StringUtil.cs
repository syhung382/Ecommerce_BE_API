using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ecommerce_BE_API.WebApi.Helpers
{
    public static class StringUtil
    {
        public static string CheckLetter(string preString, string maxValue, int length)
        {
            string yearCurrent = DateTime.Now.Year.ToString().Substring(2, 2);
            string monthCurrent = DateTime.Now.Month.ToString(); // "4"
            //khi thang hien tai nho hon 9 thi cong them "0" vao
            if (Convert.ToInt32(monthCurrent) <= 9)
            {
                monthCurrent = "0" + monthCurrent;
            }
            //Khi tham so select o database la null khoi tao so dau tien
            if (String.IsNullOrEmpty(maxValue))
            {
                string ret = "1";
                while (ret.Length < length)
                {
                    ret = "0" + ret;
                }
                return preString + yearCurrent + monthCurrent + "-" + ret;
            }
            else
            {
                string preStringMax = maxValue.Substring(0, maxValue.IndexOf("-") - 4);
                string maxNumber = maxValue.Substring(maxValue.IndexOf("-") + 1);
                string monthYear = maxValue.Substring(maxValue.IndexOf("-") - 4, 4);
                string monthDb = monthYear.Substring(2, 2); //as "04"

                string stringTemp = maxNumber;
                //Khi thang trong gia tri max bang voi thang create thi cong len cho 1
                if (monthDb == monthCurrent)
                {
                    int strToInt = Convert.ToInt32(maxNumber);
                    maxNumber = Convert.ToString(strToInt + 1);
                    while (maxNumber.Length < stringTemp.Length)
                        maxNumber = "0" + maxNumber;
                }
                else //reset
                {
                    maxNumber = "1";
                    while (maxNumber.Length < stringTemp.Length)
                    {
                        maxNumber = "0" + maxNumber;
                    }
                }

                return preStringMax + yearCurrent + monthCurrent + "-" + maxNumber;
            }
        }
        public static string GetProductID(string preString, string maxValue, int length)
        {
            //Khi tham so select o database la null khoi tao so dau tien
            if (String.IsNullOrEmpty(maxValue))
            {
                string ret = "1";
                while (ret.Length < length)
                {
                    ret = "0" + ret;
                }
                return preString + ret;
            }
            else
            {
                string maxNumber = Regex.Match(maxValue, @"\d+").Value;
                //Khi thang trong gia tri max bang voi thang create thi cong len cho 1
                int strToInt = Convert.ToInt32(maxNumber);
                maxNumber = Convert.ToString(strToInt + 1);
                while (maxNumber.Length < length)
                    maxNumber = "0" + maxNumber;

                return preString + maxNumber;
            }
        }
        public static bool Validate(string emailAddress)
        {
            if (string.IsNullOrEmpty(emailAddress))
                return false;
            var regex = @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z";
            bool isValid = Regex.IsMatch(emailAddress, regex, RegexOptions.IgnoreCase);
            return isValid;
        }
        public static string ConvertStringToDate(string date)
        {
            char[] splitter = { '/' };
            if (String.IsNullOrEmpty(date) || date == "") return null;
            string[] splitDate = date.Split(splitter);
            if (splitDate.Length != 3) return null;
            return splitDate[1].PadLeft(2, '0') + "/" + splitDate[0].PadLeft(2, '0') + "/" + splitDate[2];
        }
    }
}
