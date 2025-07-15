using Ecommerce_BE_API.DbContext.Models.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Ecommerce_BE_API.Services.Utils
{
    public static class FunctionUtils
    {
        // SHA256 Hash Code

        public static string CreateSHA256(string _secureSecret, string value)
        {
            // Hex Decode the Secure Secret for use in using the HMACSHA256 hasher
            // hex decoding eliminates this source of error as it is independent of the character encoding
            // hex decoding is precise in converting to a byte array and is the preferred form for representing binary values as hex strings. 
            byte[] convertedHash = Encoding.ASCII.GetBytes(_secureSecret);
            // Create secureHash on string
            string hexHash = "";
            using (HMACSHA256 hasher = new HMACSHA256(convertedHash))
            {
                byte[] hashValue = hasher.ComputeHash(Encoding.UTF8.GetBytes(value.ToString()));
                foreach (byte b in hashValue)
                {
                    hexHash += b.ToString("X2");
                }
            }
            return hexHash;
        }

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
        public static string CreateActionCodeCode(int userId, int randomLength = 11)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789" + DateTime.Now.ToString("yyyyMMddHHmmss");
            var userIdLength = userId.ToString().Length;
            //var randomLength = 11; // Ramdom to 8 char
            var random = new Random();
            var randomCode = new string(Enumerable.Repeat(chars, userIdLength >= randomLength ? 1 : randomLength - userIdLength)
                            .Select(s => s[random.Next(s.Length)]).ToArray());
            var inviteCode = $"{randomCode}{userId}";
            return inviteCode;
        }

        public static string CreateConstestCode(string preString, string maxValue, int length)
        {

            //Khi tham so select o database la null khoi tao so dau tien
            if (String.IsNullOrEmpty(maxValue))
            {
                string ret = "1";
                while (ret.Length < length)
                {
                    ret = "000" + ret;
                }
                return preString + ret;
            }
            else
            {
                string maxNumber = maxValue.Substring(3, maxValue.Length - 3);
                string stringTemp = maxNumber;
                //Khi thang trong gia tri max bang voi thang create thi cong len cho 1               
                int strToInt = Convert.ToInt32(maxNumber);
                maxNumber = Convert.ToString(strToInt + 1);
                if (length - maxNumber.Length == 1)
                    maxNumber = "0" + maxNumber;
                if (length - maxNumber.Length == 2)
                    maxNumber = "00" + maxNumber;
                if (length - maxNumber.Length == 3)
                    maxNumber = "000" + maxNumber;

                return preString + maxNumber;
            }
        }
        public static string CreateRefereeCode(string preString, string maxValue, int length)
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
                string maxNumber = maxValue.Substring(2, maxValue.Length - 2);
                //Khi thang trong gia tri max bang voi thang create thi cong len cho 1               
                int strToInt = Convert.ToInt32(maxNumber);
                maxNumber = Convert.ToString(strToInt + 1);
                if (length - maxNumber.Length == 1)
                    maxNumber = "0" + maxNumber;
                return preString + maxNumber;
            }
        }

        public static WeekMonthYear getWeekMonthYear(DateTime timeExec)
        {
            timeExec = DateTime.Now;
            var weekMonthYear = new WeekMonthYear();

            var cal = DateTimeFormatInfo.CurrentInfo.Calendar;
            var beginningOfMonth = new DateTime(timeExec.Year, timeExec.Month, 1);
            var currentOnWeek = cal.GetWeekOfYear(timeExec, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
            var firstOnWeek = cal.GetWeekOfYear(beginningOfMonth, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
            weekMonthYear.Week = (currentOnWeek - firstOnWeek) + 1;
            if (weekMonthYear.Week > 5) weekMonthYear.Week = 5;
            weekMonthYear.Month = timeExec.Month;
            weekMonthYear.Year = timeExec.Year;

            return weekMonthYear;
        }

        public static string ConvertToUnSign(string s)
        {
            string stFormD = s.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();
            for (int ich = 0; ich < stFormD.Length; ich++)
            {
                System.Globalization.UnicodeCategory uc = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(stFormD[ich]);
                if (uc != System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(stFormD[ich]);
                }
            }
            sb = sb.Replace('Đ', 'D');
            sb = sb.Replace('đ', 'd');
            return (sb.ToString().Normalize(NormalizationForm.FormD));
        }
        public static IQueryable<T> OrderByDynamic<T>(this IQueryable<T> q, string sortField, bool ascending)
        {
            try
            {
                var property = typeof(T).GetProperty(sortField, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (property == null)
                {
                    return q;
                }

                var parameter = Expression.Parameter(typeof(T), "p");
                var propertyAccess = Expression.Property(parameter, property.Name); 
                var orderByExp = Expression.Lambda(propertyAccess, parameter);

                string methodName = ascending ? "OrderBy" : "OrderByDescending";
                var resultExp = Expression.Call(typeof(Queryable), methodName,
                    new Type[] { typeof(T), property.PropertyType },
                    q.Expression, Expression.Quote(orderByExp));

                return q.Provider.CreateQuery<T>(resultExp);
            }
            catch
            {
                return q;
            }
        }
        public static IEnumerable<T> OrderObjectByDynamic<T>(this IEnumerable<T> source, string propertyName, bool Ascending)
        {
            try
            {
                foreach (PropertyInfo propertyInfo in source.GetType().GetProperties())
                {
                    if (propertyInfo.Name != propertyName)
                    {
                        return source;
                    }
                }
                propertyName = propertyName.Substring(0, 1).ToUpper() + propertyName.Substring(1, propertyName.Length - 1);
                if (Ascending)
                    return source.OrderBy(x => x.GetType().GetProperty(propertyName).GetValue(x, null));
                else
                    return source.OrderByDescending(x => x.GetType().GetProperty(propertyName).GetValue(x, null));
            }
            catch (Exception ex)
            {
                return source;
            }

        }

        public static bool ValidateEmail(string emailAddress)
        {
            if (string.IsNullOrEmpty(emailAddress))
                return false;
            var regex = @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z";
            bool isValid = Regex.IsMatch(emailAddress, regex, RegexOptions.IgnoreCase);
            return isValid;
        }
        public static string FormatMoneyVN(double value)
        {
            var info = System.Globalization.CultureInfo.GetCultureInfo("vi-VN");
            return String.Format(info, "{0:c}", value);
        }

        public static string RemoveVietnameseTones(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            // Normalize to FormD to split accents from letters
            string normalized = input.Normalize(NormalizationForm.FormD);

            // Remove all accent characters (diacritics)
            var regex = new Regex(@"\p{IsCombiningDiacriticalMarks}+");
            string result = regex.Replace(normalized, string.Empty);

            // Replace specific Vietnamese characters
            result = result.Replace('đ', 'd').Replace('Đ', 'D');

            // Convert to lowercase
            return result.ToLowerInvariant();
        }

        //check enum
        public static bool IsUserRoleEnum(int value)
        {
            return Enum.IsDefined(typeof(Ecommerce_BE_API.DbContext.Models.Utils.UserRoleEnum), value);
        }
        public static bool IsGenderEnum(int value)
        {
            return Enum.IsDefined(typeof(Ecommerce_BE_API.DbContext.Models.Utils.GenderEnum), value);
        }
        public static bool IsAdminRoleEnum(int value)
        {
            return Enum.IsDefined(typeof(Ecommerce_BE_API.DbContext.Models.Utils.AdminRoleEnum), value);
        }
        public static bool IsBannedEnum(int value)
        {
            return Enum.IsDefined(typeof(Ecommerce_BE_API.DbContext.Models.Utils.BannedEnum), value);
        }
        public static bool IsActivedEnum(int value)
        {
            return Enum.IsDefined(typeof(Ecommerce_BE_API.DbContext.Models.Utils.ActiveEnum), value);
        }
        public static bool IsUserStatusEnum(int value)
        {
            return Enum.IsDefined(typeof(Ecommerce_BE_API.DbContext.Models.Utils.UserStatusEnum), value);
        }
        public static bool IsStatusEnum(int value) {
            return Enum.IsDefined(typeof(Ecommerce_BE_API.DbContext.Models.Utils.StatusEnum), value);
        }
    }
}

