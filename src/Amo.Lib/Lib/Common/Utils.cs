﻿using Amo.Lib.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Amo.Lib
{
    public static class Utils
    {
        public static object ChangeType(object value, Type type)
        {
            if ((value == null) && type.IsGenericType)
            {
                return Activator.CreateInstance(type);
            }

            if (value == null)
            {
                return null;
            }

            if (type == value.GetType())
            {
                return value;
            }

            if (type.IsEnum)
            {
                if (value is string)
                {
                    return Enum.Parse(type, value as string);
                }

                return Enum.ToObject(type, value);
            }

            if (!type.IsInterface && type.IsGenericType)
            {
                Type type1 = type.GetGenericArguments()[0];
                object obj1 = ChangeType(value, type1);
                return Activator.CreateInstance(type, new object[] { obj1 });
            }

            if ((value is string) && (type == typeof(Guid)))
            {
                return new Guid(value as string);
            }

            if ((value is string) && (type == typeof(Version)))
            {
                return new Version(value as string);
            }

            if (!(value is IConvertible))
            {
                return value;
            }

            return Convert.ChangeType(value, type);
        }

        public static int GetInt(object obj)
        {
            try
            {
                return Convert.ToInt32(obj);
            }
            catch
            {
                return 0;
            }
        }

        public static string GetString(object obj)
        {
            try
            {
                if (obj == System.DBNull.Value)
                {
                    return string.Empty;
                }
                else
                {
                    return obj.ToString();
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        public static bool GetBoolean(object obj)
        {
            try
            {
                return Convert.ToBoolean(obj);
            }
            catch
            {
                return false;
            }
        }

        public static double GetDouble(object obj)
        {
            try
            {
                return Convert.ToDouble(obj);
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// 描述：四舍五入函数
        /// </summary>
        /// <remarks>
        /// 用途：将一个小数进行四舍五入。
        /// </remarks>
        /// <param name="ans">将要进行四舍五入的数</param>
        /// <param name="len">要保留的小数位数</param>
        /// <returns>四舍五入完成的数</returns>
        public static decimal Round(decimal ans, int len)
        {
            for (int i = 0; i < len; i++)
            {
                ans *= 10M;
            }

            if (ans >= 0)
            {
                ans = Math.Floor(ans + 0.5M);
            }
            else
            {
                ans = Math.Ceiling(ans - 0.5M);
            }

            for (int i = 0; i < len; i++)
            {
                ans /= 10M;
            }

            return ans;
        }

        /// <summary>
        /// 正整数求百分比函数
        /// </summary>
        /// <param name="numerator">分子</param>
        /// <param name="denominator">分母</param>
        /// <returns>百分比</returns>
        public static string Percent(int numerator, int denominator)
        {
            if (numerator <= 0 || denominator <= 0)
            {
                return "0.00%";
            }

            return ((numerator / (double)denominator) * 100).ToString("f2") + "%";
        }

        /// <summary>
        /// 判断是否为整数
        /// </summary>
        /// <param name="strInt">字符串</param>
        /// <returns>是否为整数</returns>
        public static bool IsInt(string strInt)
        {
            try
            {
                if (string.IsNullOrEmpty(strInt))
                {
                    return false;
                }

                char[] charList = strInt.ToCharArray();
                foreach (char c in charList)
                {
                    if (!char.IsDigit(c))
                    {
                        return false;
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 将描述转换成下划线连接的Url
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="hyphen">连字符</param>
        /// <param name="lowerOrUpper">0:不变,1:Lower,2:Upper</param>
        /// <returns>url</returns>
        public static string DescToUrl(string url, string hyphen = "_", int lowerOrUpper = 0)
        {
            if (url == null)
            {
                return string.Empty;
            }

            switch (lowerOrUpper)
            {
                case 0: break;
                case 1: url = url.ToLower(); break;
                case 2: url = url.ToUpper(); break;
                default: break;
            }

            string newStr = GetDealtText(url);
            string[] array = newStr.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            return string.Join(hyphen, array);
        }

        /// <summary>
        /// 通过正则表达式从URL中拆出参数
        /// </summary>
        /// <param name="input">URL的相对路径部分</param>
        /// <param name="regex">正则表达式</param>
        /// <returns>以KeyValuePair的形式，返回参数及参数值</returns>
        public static GroupCollection GetMatches(string input, string regex)
        {
            if (!Regex.IsMatch(input, regex))
            {
                return null;
            }

            // 定义正则表达式，及几个可选项
            Regex textRegex = new Regex(regex, RegexOptions.ExplicitCapture | RegexOptions.Multiline | RegexOptions.IgnoreCase);

            // 匹配URL，并从URL中捕获相应标签的值
            MatchCollection matchItems = textRegex.Matches(input);

            if (matchItems.Count == 1)
            {
                return matchItems[0].Groups;
            }

            return null;
        }

        /// <summary>
        /// 解析Vin中的年份
        /// </summary>
        /// <param name="digitYear">Vin中的第十位</param>
        /// <param name="maxYear">当前年份(30年一循环)</param>
        /// <returns>Vin的年份</returns>
        public static int DecodeVinYear(char digitYear, int maxYear)
        {
            int year = -1;

            // 拼接出所有年份的Char字符串,A=1980,后面递加
            string vinYears = "ABCDEFGHJKLMNPRSTVWXY123456789";
            if (vinYears.IndexOf(digitYear) > -1)
            {
                // 当前字符的初始年份
                year = 1980 + vinYears.IndexOf(digitYear);

                // 因为年份是30年一循环,所以不可以距离当前年份超过30
                while (maxYear - year >= 30)
                {
                    year = year + 30;
                }
            }

            return year;
        }

        /// <summary>
        /// 去掉零件中除了数字、字母以外的其他符号
        /// </summary>
        /// <param name="str">输入字符串</param>
        /// <param name="trimStartZero">是否去除前置0</param>
        /// <returns>转换后的字符串</returns>
        public static string GetSearchPartNumber(string str, bool trimStartZero = false)
        {
            return GetSearchPartNumber(str, 1, trimStartZero, "0");
        }

        /// <summary>
        /// 去掉零件中除了数字、字母以外的其他符号
        /// </summary>
        /// <param name="str">输入字符串</param>
        /// <param name="lowerOrUpper">0:不变,1:Lower,2:Upper</param>
        /// <param name="trimStartChar">是否去除前置字符</param>
        /// <param name="trimChar">前置字符</param>
        /// <returns>转换后的字符串</returns>
        public static string GetSearchPartNumber(string str, int lowerOrUpper = 0, bool trimStartChar = false, string trimChar = "")
        {
            if (str == null)
            {
                return string.Empty;
            }

            switch (lowerOrUpper)
            {
                case 0: break;
                case 1: str = str.ToLower(); break;
                case 2: str = str.ToUpper(); break;
                default: break;
            }

            string newStr = GetDealtText(str).Replace(" ", string.Empty);

            if (trimStartChar && !string.IsNullOrEmpty(trimChar))
            {
                newStr = newStr.TrimStart(trimChar.ToCharArray());
            }

            return newStr;
        }

        /// <summary>
        /// 获取路径下的所有文件
        /// </summary>
        /// <param name="filePath">初始路径</param>
        /// <returns>文件列表</returns>
        public static List<string> LoadFolderFile(string filePath)
        {
            List<string> outfileList = new List<string>();

            // 文件夹
            string[] dirs = Directory.GetDirectories(filePath);

            // 文件
            string[] files = Directory.GetFiles(filePath);

            for (int i = 0; i < dirs.Length; i++)
            {
                DirectoryInfo di = new DirectoryInfo(dirs[i]);
                try
                {
                    if (di.GetDirectories().Length > 0 || di.GetFiles().Length > 0)
                    {
                        outfileList.AddRange(LoadFolderFile(di.FullName));
                    }
                    else
                    {
                        continue;
                    }
                }
                catch
                {
                    continue;
                }
            }

            for (int i = 0; i < files.Length; i++)
            {
                FileInfo fi = new FileInfo(files[i]);
                outfileList.Add(fi.FullName);
            }

            return outfileList;
        }

        public static string CutStr(string inputStr, string splitStr, bool firstOrLast = false, bool previousOrNext = false)
        {
            if (string.IsNullOrEmpty(inputStr))
            {
                return null;
            }

            if (string.IsNullOrEmpty(splitStr))
            {
                return inputStr;
            }

            string str = null;
            int idx = -1;
            if (firstOrLast)
            {
                idx = inputStr.IndexOf(splitStr);
            }
            else
            {
                idx = inputStr.LastIndexOf(splitStr);
            }

            if (idx > -1)
            {
                if (previousOrNext)
                {
                    str = inputStr.Substring(0, idx);
                }
                else
                {
                    str = inputStr.Substring(idx + 1);
                }
            }
            else
            {
                if (previousOrNext)
                {
                    str = inputStr;
                }
            }

            return str;
        }

        /// <summary>
        /// 计算错误类型
        /// 倒数第四位为Level标记位,1-6对应Level,其他默认Info
        /// <seealso cref="Enums.EventType"/>
        /// </summary>
        /// <param name="eventType">EventType</param>
        /// <returns>Level</returns>
        public static LogLevel GetLevel(int eventType)
        {
            if (eventType < 100000)
            {
                return LogLevel.Info;
            }

            int key = (eventType / 1000) % 10;

            LogLevel level;
            switch (key)
            {
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                    level = (LogLevel)key;
                    break;
                default:
                    level = LogLevel.Info;
                    break;
            }

            return level;
        }

        /// <summary>
        /// 屏蔽字符中除了数字、字母以外的其他符号(换成空)
        /// </summary>
        /// <param name="text">input str</param>
        /// <returns>str</returns>
        private static string GetDealtText(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            char[] charList = text.ToCharArray();
            for (int i = 0; i < charList.Length; i++)
            {
                if (!char.IsLetterOrDigit(charList[i]))
                {
                    charList[i] = ' ';
                }
            }

            string dealtText = new string(charList);
            return dealtText;
        }
    }
}
