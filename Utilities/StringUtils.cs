using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Mail;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace DVG.WIS.Utilities
{
    public class StringUtils
    {
        public static string CalculateMD5Hash(string input)
        {
            // step 1, calculate MD5 hash from input
            MD5 md5 = MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }

        public static string QuoteString(string inputString)
        {
            string str = inputString.Trim();
            if (str != "")
            {
                str = str.Replace("'", "''");
            }
            return str;
        }

        public static string AddSlash(string input)
        {
            string str = !string.IsNullOrEmpty(input) ? input.Trim() : "";
            if (str != "")
            {
                str = str.Replace("'", "'").Replace("\"", "\\\"");
            }
            return str;
        }

        public static string RefreshText(string text)
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(text.Trim())) return text;

            text = HttpUtility.HtmlDecode(text);

            text = HttpUtility.UrlDecode(text);

            return text;
        }

        public static string RemoveStrHtmlTags(object inputObject)
        {
            if (inputObject == null)
            {
                return string.Empty;
            }
            string input = Convert.ToString(inputObject).Trim();
            if (input != "")
            {
                input = Regex.Replace(input, @"<(.|\n)*?>", string.Empty);
            }
            return input;
        }

        public static string ReplaceSpaceToPlus(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                return Regex.Replace(input, @"\s+", "+", RegexOptions.IgnoreCase);
            }
            return input;
        }

        public static string ReplaceSpecialCharater(object inputObject)
        {
            if (inputObject == null)
            {
                return string.Empty;
            }
            return Convert.ToString(inputObject).Trim().Trim().Replace(@"\", @"\\").Replace("\"", "&quot;").Replace("“", "&ldquo;").Replace("”", "&rdquo;").Replace("‘", "&lsquo;").Replace("’", "&rsquo;").Replace("'", "&#39;");
        }

        public static string JavaScriptSring(string input)
        {
            input = input.Replace("'", @"\u0027");
            input = input.Replace("\"", @"\u0022");
            return input;
        }

        public static int CountWords(string stringInput)
        {
            if (string.IsNullOrEmpty(stringInput))
            {
                return 0;
            }
            stringInput = RemoveStrHtmlTags(stringInput);
            return Regex.Matches(stringInput, @"[\S]+").Count;
        }

        public static string GetEnumDescription(Enum value)
        {
            try
            {
                FieldInfo fi = value.GetType().GetField(value.ToString());

                DescriptionAttribute[] attributes =
                    (DescriptionAttribute[])fi.GetCustomAttributes(
                        typeof(DescriptionAttribute),
                        false);

                if (attributes != null &&
                    attributes.Length > 0)
                    return attributes[0].Description;
                else
                    return value.ToString();
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        public static string GetPropertyDisplayName<T>(Expression<Func<T, object>> propertyExpression)
        {
            var memberInfo = GetPropertyInformation(propertyExpression.Body);
            if (memberInfo == null)
            {
                throw new ArgumentException(
                    "No property reference expression was found.",
                    "propertyExpression");
            }

            var attr = memberInfo.GetAttribute<DisplayNameAttribute>(false);
            if (attr == null)
            {
                return memberInfo.Name;
            }

            return attr.DisplayName;
        }

        public static MemberInfo GetPropertyInformation(Expression propertyExpression)
        {
            //Debug.Assert(propertyExpression != null, "propertyExpression != null");
            MemberExpression memberExpr = propertyExpression as MemberExpression;
            if (memberExpr == null)
            {
                UnaryExpression unaryExpr = propertyExpression as UnaryExpression;
                if (unaryExpr != null && unaryExpr.NodeType == ExpressionType.Convert)
                {
                    memberExpr = unaryExpr.Operand as MemberExpression;
                }
            }

            if (memberExpr != null && memberExpr.Member.MemberType == MemberTypes.Property)
            {
                return memberExpr.Member;
            }

            return null;
        }

        public static string SubWordInString(object obj, int maxWord, bool removeHTML = false)
        {
            if (obj == null)
            {
                return string.Empty;
            }

            if (removeHTML) obj = RemoveStrHtmlTags(obj);

            string input = Regex.Replace(Convert.ToString(obj), @"\s+", " ");


            string[] strArray = Regex.Split(input, " ");
            if (strArray.Length <= maxWord)
            {
                return input;
            }
            input = string.Empty;
            for (int i = 0; i < maxWord; i++)
            {
                input = input + strArray[i] + " ";
            }
            return string.Concat(input.Trim(), "...");
        }

        public static string SubWordInDotString(object obj, int maxWord, string extensionEnd = " ...")
        {
            if (obj == null)
            {
                return string.Empty;
            }
            string input = Regex.Replace(Convert.ToString(obj), @"\s+", " ");
            string[] strArray = Regex.Split(input, " ");
            if (strArray.Length <= maxWord)
            {
                return input;
            }
            input = string.Empty;
            for (int i = 0; i < maxWord; i++)
            {
                input = input + strArray[i] + " ";
            }
            return (input.Trim() + extensionEnd);
        }

        public static string StripHtml(string html)
        {
            return (string.IsNullOrEmpty(html) ? string.Empty : Regex.Replace(html, "<.*?>", string.Empty));
        }
        public static string TrimText(object strIn, int intLength)
        {
            try
            {
                string str = StripHtml(Convert.ToString(strIn));
                if (str.Length > intLength)
                {
                    str = str.Substring(0, intLength - 4);
                    return (str.Substring(0, str.LastIndexOfAny(new char[] { ' ', '.', '?', ',', '!' })) + " ...");
                }
                return str;
            }
            catch (Exception)
            {
                return Convert.ToString(strIn);
            }
        }

        public static string FormatNumber(string sNumber, string sperator = ".")
        {
            int num = 3;
            int num2 = 0;
            for (int i = 1; i <= (sNumber.Length / 3); i++)
            {
                if ((num + num2) < sNumber.Length)
                {
                    sNumber = sNumber.Insert((sNumber.Length - num) - num2, sperator);
                }
                num += 3;
                num2++;
            }
            return sNumber;
        }

        public static string FormatNumberWithComma(string sNumber)
        {
            int num = 3;
            int num2 = 0;
            for (int i = 1; i <= (sNumber.Length / 3); i++)
            {
                if ((num + num2) < sNumber.Length)
                {
                    sNumber = sNumber.Insert((sNumber.Length - num) - num2, ",");
                }
                num += 3;
                num2++;
            }
            return sNumber;
        }

        public static bool IsValidWord(string input, char character)
        {
            if (string.IsNullOrEmpty(input))
            {
                return true;
            }
            string[] arr = input.Split(character);
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i].Length > 30)
                {
                    return false;
                }
            }
            return true;
        }

        public static bool IsValidEmail(string emailaddress)
        {
            try
            {
                MailAddress m = new MailAddress(emailaddress);

                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        public static string GetMetaDescription(string format, params object[] args)
        {
            if (String.IsNullOrEmpty(format)) return String.Empty;

            string strDes = format;

            if (!String.IsNullOrEmpty(format) && args != null && args.Length > 0)
            {
                strDes = String.Format(strDes, args);
            }

            return strDes;
        }

        public static string ConvertNumberToCurrency(double number, string sperator = ".", string currentcy = "")
        {
            if (number <= 0)
            {
                return "0";
            }

            number = Math.Round(number, 0);

            string output = StringUtils.FormatNumber(number.ToString(CultureInfo.CurrentCulture), sperator) + currentcy;

            return output;
        }

        public static string ConvertNumberToCurrency2(double number, string sperator = ".", string currentcy = "")
        {
            string output = string.Empty;
            if (number < 0)
            {
                number = number * (-1);
                number = Math.Round(number, 0);

                output = "-" + StringUtils.FormatNumber(number.ToString(CultureInfo.CurrentCulture), sperator) + currentcy;

                return output;
            }

            number = Math.Round(number, 0);

            output = StringUtils.FormatNumber(number.ToString(CultureInfo.CurrentCulture), sperator) + currentcy;

            return output;
        }

        public static string ReplaceCaseInsensitive(string input, string[] search, string[] replacement)
        {
            int lenSearch = search.Length, lenRepalace = replacement.Length;
            string result = string.Empty;
            for (int i = 0; i < lenSearch; i++)
            {
                for (int j = 0; j < lenRepalace; j++)
                {
                    result = Regex.Replace(
                        input,
                        Regex.Escape(search[i]),
                        replacement[j].Replace("$", "$$"),
                        RegexOptions.IgnoreCase
                    ).Trim();
                    input = result;
                }
            }

            return result;
        }

        public static string GetStringTreeview(int level)
        {
            if (level == 0) return string.Empty;

            string strLevel = "";
            for (int i = 0; i < level; i++)
            {
                strLevel = strLevel + "__ ";
            }
            return strLevel;
        }

        #region Content Process

        public static string UploadImageIncontent(string content, out string firstImage, Func<string, string> uploadImage)
        {
            firstImage = String.Empty;

            string newContent = content;

            if (String.IsNullOrEmpty(newContent)) return newContent;

            try
            {

                string strRegex = @"<img.+?src=[\""'](?<SRC>.+?)[\""'].*?>";
                Regex myRegex = new Regex(strRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);

                Match match = myRegex.Match(newContent);
                if (match.Success)
                {
                    newContent = myRegex.Replace(newContent, m => string.Format("<p style=\"text-align:center\"><img src=\"{0}\" /></p>", uploadImage.Invoke(m.Groups["SRC"].Value)));
                }


                foreach (Match matchAvatar in myRegex.Matches(newContent))
                {
                    if (matchAvatar.Success)
                    {
                        firstImage = matchAvatar.Groups["SRC"].Value;
                        break;
                    }
                }

            }
            catch (Exception ex)
            {
                // Todo
            }
            return newContent;
        }

        #endregion

        public static string AddAttributeForAnchors(string htmlContent, string domainTarget = "http://banxehoi.com/diendan/seolink/?refer=", bool isEncrypt = false)
        {
            if (string.IsNullOrEmpty(htmlContent)) return htmlContent;
            try
            {
                htmlContent = Regex.Replace(htmlContent, @"rel=[""']nofollow[""']", string.Empty);
                htmlContent = htmlContent.Replace(@"target=[""']_blank[""']", string.Empty);

                string strRegex = @"(?<LINK><a[^>]href=[""'](?<url>[^""']+)[""'](?<attrs>[^>]*)>(?<Content>((?!<\/a>).)*)<\/a>)";
                Regex myRegex = new Regex(strRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                Match match = myRegex.Match(htmlContent);
                //string strReplace = @"<a href=""" + domainTarget + @"${url}"" ${attrs} rel=""nofollow"" target=""_blank"">${Content}</a>";

                if (match.Success)
                {
                    htmlContent = myRegex.Replace(htmlContent, delegate (Match m)
                    {
                        string url = domainTarget + m.Groups["url"].Value;
                        string attrs = m.Groups["attrs"].Value;
                        string content = m.Groups["Content"].Value;
                        string link = string.Format(@"<a href=""{0}"" {1} rel=""nofollow"" target=""_blank"">{2}</a>", url, attrs,
                            content);
                        link = Regex.Replace(link, @"\s+", " ");
                        return link;
                    });
                    //htmlContent = myRegex.Replace(htmlContent, strReplace);
                }

            }
            catch
            {
                // Todo something
            }
            return htmlContent;
        }

        #region Unicode Process

        public const string uniChars =
            "àáảãạâầấẩẫậăằắẳẵặèéẻẽẹêềếểễệđìíỉĩịòóỏõọôồốổỗộơờớởỡợùúủũụưừứửữựỳýỷỹỵÀÁẢÃẠÂẦẤẨẪẬĂẰẮẲẴẶÈÉẺẼẸÊỀẾỂỄỆĐÌÍỈĨỊÒÓỎÕỌÔỒỐỔỖỘƠỜỚỞỠỢÙÚỦŨỤƯỪỨỬỮỰỲÝỶỸỴÂĂĐÔƠƯ";

        public const string unsignChar =
            "aaaaaaaaaaaaaaaaaeeeeeeeeeeediiiiiooooooooooooooooouuuuuuuuuuuyyyyyAAAAAAAAAAAAAAAAAEEEEEEEEEEEDIIIIIOOOOOOOOOOOOOOOOOUUUUUUUUUUUYYYYYAADOOU";

        public static string UnicodeToUnsignChar(string s)
        {
            if (string.IsNullOrEmpty(s)) return s;
            string retVal = String.Empty;
            int pos;
            for (int i = 0; i < s.Length; i++)
            {
                pos = uniChars.IndexOf(s[i].ToString());
                if (pos >= 0)
                    retVal += unsignChar[pos];
                else
                    retVal += s[i];
            }
            return retVal;
        }

        public static string UnicodeToUnsignCharAndDash(string s)
        {
            if (string.IsNullOrEmpty(s)) return string.Empty;
            const string strChar = "abcdefghijklmnopqrstxyzuvxw0123456789 -";
            //string retVal = UnicodeToKoDau(s);
            s = UnicodeToUnsignChar(s.ToLower().Trim());
            string sReturn = "";
            for (int i = 0; i < s.Length; i++)
            {
                if (strChar.IndexOf(s[i]) > -1)
                {
                    if (s[i] != ' ')
                        sReturn += s[i];
                    else if (i > 0 && s[i - 1] != ' ' && s[i - 1] != '-')
                        sReturn += "-";
                }
            }
            while (sReturn.IndexOf("--") != -1)
            {
                sReturn = sReturn.Replace("--", "-");
            }
            return sReturn;
        }
        public static string RemoveSpecial(string s)
        {
            //const string REGEX = @"([^\w\dàáảãạâầấẩẫậăằắẳẵặèéẻẽẹêềếểễệđìíỉĩịòóỏõọôồốổỗộơờớởỡợùúủũụưừứửữựỳýỷỹỵÀÁẢÃẠÂẦẤẨẪẬĂẰẮẲẴẶÈÉẺẼẸÊỀẾỂỄỆĐÌÍỈĨỊÒÓỎÕỌÔỒỐỔỖỘƠỜỚỞỠỢÙÚỦŨỤƯỪỨỬỮỰỲÝỶỸỴÂĂĐÔƠƯ\.,\-_ ]+)";
            //s = Regex.Replace(s, REGEX, string.Empty, RegexOptions.IgnoreCase);

            return Regex.Replace(s, "[`~!@#$%^&*()_|+-=?;:'\"<>{}[]\\/]", string.Empty); //edited by vinhph

        }
        public static string RemoveSpecial4ModelDetail(string s)
        {
            string result = string.Empty;
            if (!string.IsNullOrEmpty(s))
            {
                result = Regex.Replace(s, "[+*%/^&:]", string.Empty, RegexOptions.IgnoreCase);
            }
            return result;
        }
        public static string ReplaceSpecial4ModelDetail(string s)
        {
            string result = string.Empty;
            result = Regex.Replace(s, "plus", "+", RegexOptions.IgnoreCase);
            result = Regex.Replace(result, "star", "*", RegexOptions.IgnoreCase);
            result = Regex.Replace(result, "per", "%", RegexOptions.IgnoreCase);
            return result;
        }
        public static string UnicodeToKoDauAndSpace(string s)
        {
            if (string.IsNullOrEmpty(s)) return string.Empty;
            string retVal = String.Empty;
            int pos;
            for (int i = 0; i < s.Length; i++)
            {
                pos = uniChars.IndexOf(s[i].ToString());
                if (pos >= 0)
                    retVal += unsignChar[pos];
                else
                    retVal += s[i];
            }
            return retVal;
        }
        /// <summary>
        /// loại bỏ các ký tự không phải chữ, số, dấu cách thành ký tự không dấu
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string RemoveSpecialCharToKhongDau(string s)
        {
            string retVal = UnicodeToUnsignChar(s);
            Regex regex = new Regex(@"[^\d\w]+");
            retVal = regex.Replace(retVal, " ");
            while (retVal.IndexOf("  ") != -1)
            {
                retVal = retVal.Replace("  ", " ");
            }
            return retVal;
        }

        #endregion

        //static char[] baseChars = new char[36] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
        static char[] baseChars = new char[34] { '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
        const int Base36Size = 34;

        /// <summary>
        /// The int to base36
        /// Author: ThanhDT
        /// Created date: 8/17/2020 8:35 AM
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string IntToBase36(long value)
        {
            StringBuilder sbNumber = new StringBuilder();
            do
            {
                sbNumber.Insert(0, baseChars[value % Base36Size]);
                value = value / Base36Size;
            }
            while (value > 0);

            return sbNumber.ToString();
        }

        /// <summary>
        /// The generate unique identifier
        /// Author: ThanhDT
        /// Created date: 8/17/2020 8:36 AM
        /// </summary>
        /// <returns></returns>
        public static string GenerateUniqueId()
        {
            var unixTimestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            var uniqueId = IntToBase36(unixTimestamp);

            return uniqueId;
        }

        public static string ComboFlowMessage(string mess, int orderId)
        {
            return string.Format("[Combo] ID:{1} - {0}", mess, orderId);
        }

        public static string ProductFlowMessage(string mess, int orderId)
        {
            return string.Format("[Product] ID:{1} - {0}", mess, orderId);
        }

        public static string OrderFlowMessage(string mess, int orderId)
        {
            return string.Format("[Order] ID:{1} - {0}", mess, orderId);
        }

        public static string PromotionFlowMessage(string mess, int id)
        {
            return string.Format("[Promotion] ID:{1} - {0}", mess, id);
        }

        public static string PromotionFlowMessage(string mess, string id)
        {
            return string.Format("[Promotion] {1} - {0}", mess, id);
        }

        public static string GrabnowFlowMessage(string mess, string id)
        {
            return string.Format("[Grabnow] {1} - {0}", mess, id);
        }

        public static string StockBalanceFlowMessage(string mess, int kitchenId, int productId, int order = 0)
        {
            return string.Format("[StockBalance] Kitchen:{0} - Product:{1} - Order:{2} - {3}", mess, kitchenId, productId, order, mess);
        }
        public static string ShipperMessage(string mess, string account)
        {
            return string.Format("[Shipper] {0} - Shipper:{1} ", mess, account);
        }

        public static string MoneyTransactionFlowMessage(string mess, int TransactionType)
        {
            return string.Format("[MoneyTransaction] {0} - {1}", mess, TransactionType);
        }
        public static string ShipperReportMessage(string mess, string account)
        {
            return string.Format("[ShipperReport] {0} - Shipper:{1} ", mess, account);
        }
        public static string FileManagerMessage(string mess, string account)
        {
            return string.Format("[FileManager] {0} - Shipper:{1} ", mess, account);
        }
        public static string AccountMessage(string mess, string account)
        {
            return string.Format("[Account] {0} - Account:{1} ", mess, account);
        }
    }
}
