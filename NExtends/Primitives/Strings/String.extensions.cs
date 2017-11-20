using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;


namespace NExtends.Primitives.Strings
{
    public static class StringExtensions
    {
        private const string DEFAULT_ELLIPSIS = "...";

        public static DateTime ParseJsonDate(this string date)
        {
            DateTime result;

            //http://stackoverflow.com/questions/5521553/best-way-to-convert-javascript-date-to-net-date
            if (!DateTime.TryParseExact(date.Substring(0, 24), "ddd MMM d yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out result))
            {
                //Ie, of course
                //http://stackoverflow.com/questions/1877788/javascript-date-to-c-sharp-via-ajax pour ie
                result = DateTime.ParseExact(date, "ddd MMM d HH:mm:ss UTCzzzzz yyyy", CultureInfo.InvariantCulture);
            }

            return result;
        }

        public static string Format(this string s, params object[] args)
        {
            return args != null ? string.Format(s, args) : s;
        }
         
        //Validation des numéro de sécu INSEE
        //version de gabsoftware
        //http://www.developpez.net/forums/d677820/php/langage/regex/verification-numero-securite-sociale/
        private const string REGULAR_INSEE_NUMBER = @"([1-3])[\s\.\-]?(\d{2})[\s\.\-]?(0\d|[2-35-9]\d|[14][0-2])[\s\.\-]?(0[1-9]|[1-9]\d|2[abAB])[\s\.\-]?(\d{3})[\s\.\-]?(\d{3})[\s\.\-\|]{0,3}";
        private const string TEMPORARY_INSEE_NUMBER = @"(?<temp>[7-9][\s\.\-]?\d{2}[\s\.\-]?\d{2}[\s\.\-]?\d{2}[\s\.\-]?\d{3}[\s\.\-]?\d{3}[\s\.\-\|]?)";
        private const string KEY_INSEE_NUMBER = @"(?<key>[0-8]\d|9[0-7])?";

        static Regex isINSEENumber = new Regex(String.Format(@"^(({0})|({1})){2}$", REGULAR_INSEE_NUMBER, TEMPORARY_INSEE_NUMBER, KEY_INSEE_NUMBER), RegexOptions.Compiled);

        public static bool IsGuid(this string input) => Guid.TryParse(input, out Guid result);

        public static bool IsINSEENumber(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }

            var match = isINSEENumber.Match(input);
            if (!match.Success)
            {
                return false;
            }

            if (!string.IsNullOrEmpty(match.Groups["key"].Value))
            {
                var number = Int64.Parse(string.Join("", input.Split(' ', '.', '-', '|')).Replace("2a", "19", StringComparison.OrdinalIgnoreCase).Replace("2b", "18", StringComparison.OrdinalIgnoreCase));
                var rest = ((number / 100) + number % 100) % 97;
                if (rest != 0)
                {
                    return false;
                }
            }
            else if (!string.IsNullOrEmpty(match.Groups["temp"].Value))
            {
                // Si le numéro est de type temporaire, on impose la clé (donc ça doit passer dans le if au-dessus)
                return false;
            }

            return true;
        }

        /// <summary>
        /// On passe par un MailAddress, plus simple
        /// </summary>
        public static bool isEmail(this string email)
        {
            //https://stackoverflow.com/a/1374644/533686
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public static bool ContainsIgnoreCase(this string source, string value)
        {
            return source.IndexOf(value, StringComparison.OrdinalIgnoreCase) != -1;
        }

        public static String SQLProtect(this String value)
        {
            return value.Replace("'", "''");
        }

        public static String ToFirstUpper(this String word)
        {
            if (!String.IsNullOrEmpty(word))
            {
                return word[0].ToString().ToUpper() + word.Substring(1);
            }
            else
            {
                return word;
            }
        }

        public static String JSONProtect(this String value)
        {
            if (value != null)
            {
                String json = value.Replace("\r\n", "<br>");
                json = json.Replace("\n", "<br>");
                json = json.Replace("\r", "<br>");

                json = json.Replace("\\", "\\\\");

                json = json.Replace("'", "\\'");
                json = json.Replace("\"", "\\\"");

                return json;
            }
            else
            {
                return "";
            }
        }

        public static String JSONQuotesProtect(this String value)
        {
            if (value != null)
            {
                String json = value.Replace("\r\n", "<br>");
                json = json.Replace("\n", "<br>");
                json = json.Replace("\r", "<br>");

                json = json.Replace("\\", "\\\\");

                json = json.Replace("\"", "\\\"");

                return json;
            }
            else
            {
                return "";
            }
        }

        //Quelqueq méthodes pour convertir des types basiques en JSON
        public static String ToJSON(this bool b)
        {
            return b.ToString().ToLower();
        }
        public static String ToJSON(this String s)
        {
            return JsonConvert.SerializeObject(s);
        }
        public static String ToDoubleQuotesJSON(this String s)
        {
            return JsonConvert.SerializeObject(s);
        }
        public static String ToDoubleQuotesJSON(this int i)
        {
            return i.ToJSON();
        }
        public static String ToDoubleQuotesJSON(this double d)
        {
            return d.ToJSON();
        }
        public static String ToDoubleQuotesJSON(this DateTime d)
        {
            return d.ToJQuery();
        }
        public static String ToDoubleQuotesJSON(this bool b)
        {
            return b.ToJSON();
        }
        public static String ToDoubleQuotesJSON(this object o)
        {
            if (o != null)
            {
                if (o is int)
                {
                    return ((int)o).ToDoubleQuotesJSON();
                }

                if (o is double)
                {
                    return ((double)o).ToDoubleQuotesJSON();
                }

                if (o is DateTime)
                {
                    return ((DateTime)o).ToDoubleQuotesJSON();
                }

                var oUrl = o as Uri;
                if (oUrl != null)
                {
                    return oUrl.ToString().ToDoubleQuotesJSON();
                }

                if (o is string)
                {
                    return o.ToString().ToDoubleQuotesJSON();
                }

                if (o is bool)
                {
                    return ((bool)o).ToDoubleQuotesJSON();
                }

                throw new NotImplementedException();
            }

            return "null";
        }

        public static Dictionary<TKey, TValue> SplitPipeValues<TKey, TValue>(this String str)
        {
            if (str != null && str.Contains("|"))
            {
                return Regex.Split(str, "\\|\\|").ToDictionary
                (
                    KV => (TKey)Convert.ChangeType(KV.Split('|')[0], typeof(TKey)),
                    KV => (TValue)Convert.ChangeType(KV.Split('|')[1], typeof(TValue))
                );
            }

            return new Dictionary<TKey, TValue>();
        }

        public static Dictionary<String, String> SplitValues(this String KVPValues)
        {
            return KVPValues.SplitPipeValues<string, string>();
        }

        public static String ToXML(this String s)
        {
            return "<![CDATA[" + s + "]]>";
        }
        public static String XMLProtect(this String value)
        {
            String xml = "<![CDATA[" + value + "]]>";
            return xml;
        }

        // In order to be able to do a Contains() using ignorecase
        // => str.Contains("x", StringComparison.InvariantCultureIgnoreCase)
        public static Boolean Contains(this String source, String value, StringComparison comparisonType)
        {
            return source.IndexOf(value, comparisonType) >= 0;
        }

        //Permet de faire un Split et de retourner un tableau vide quand la chaine de départ est vide
        public static string[] Split(this string input, string pattern)
        {
            if (String.IsNullOrEmpty(input))
            {
                return new string[] { };
            }
            else
            {
                return Regex.Split(input, pattern);
            }
        }

        /// <summary>
        /// http://stackoverflow.com/questions/249087/how-do-i-remove-diacritics-accents-from-a-string-in-net
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string RemoveDiacritics(this string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        public static string RemoveSpecialCaracters(this string text)
        {
            return Regex.Replace(text, @"[^a-zA-Z]+", "");
        }

        private static Dictionary<string, string> SanitizeSpecialCaractersMap = new Dictionary<string, string>()
        {
            {"Ø","O"},
            {"ø","o"},
            {"æ","ae"},
            {"Æ","AE"},
            {"œ","oe"},
            {"Œ","OE"}
        };

        /// <summary>
        /// https://stackoverflow.com/questions/1946997/regex-replace-multiple-characters
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string SanitizeSpecialCaracters(this string text)
        {
            var exp = new Regex(@"[ØøæÆœŒ]");

            return exp.Replace(text, (Match m) => SanitizeSpecialCaractersMap[m.Value]);
        }

        //Pour répéter des String xx fois
        public static String Repeat(this String s, int count)
        {
            if (count > 1) //Plus d'une fois
            {
                return String.Concat(s, s.Repeat(count - 1));
            }
            else if (count == 1) //1 fois
            {
                return s;
            }
            else //0 fois
            {
                return "";
            }
        }

        /// <summary>
        /// http://stackoverflow.com/questions/228038/best-way-to-reverse-a-string
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string Reverse(this string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        public static String Right(this String value, int count)
        {
            return value.Substring(Math.Max(0, value.Length - count));
        }
        /// <summary>
        /// Tronque la chaîne de caractères à la longueur souhaitée, en ajoutant éventuellement une ellipse 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="maxLength"></param>
        /// <param name="useEllipsis"></param>
        /// <param name="ellipsis"></param>
        /// <returns></returns>
        public static String Truncate(this String text, int maxLength, bool useEllipsis = true, string ellipsis = DEFAULT_ELLIPSIS)
        {
            if (String.IsNullOrEmpty(text)) return string.Empty;
            if (text.Length < maxLength) return text;

            text = text.Substring(0, useEllipsis ? maxLength - ellipsis.Length : maxLength);

            if (useEllipsis)
                text += ellipsis;

            return text;
        }

        public static Double ToDouble(this String d)
        {
            if (!String.IsNullOrEmpty(d))
            {
                return Convert.ToDouble(d.Replace(",", "."), CultureInfo.InvariantCulture);
            }

            return 0;
        }
        public static String ToHTML(this String value)
        {
            if (value != null)
            {
                String html = value.Replace("\r\n", "<br>");
                html = html.Replace("\n", "<br>");
                html = html.Replace("\r", "<br>");

                return html;
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// Equivalent à Convert.ChangeType mais gère mieux les Enum et les TimeSpan
        /// </summary>
        /// <param name="value"></param>
        /// <param name="propertyType"></param>
        /// <returns></returns>
        public static object ChangeType(this string value, Type propertyType, IFormatProvider culture)
        {
            if (propertyType.GetTypeInfo().IsEnum)
            {
                return Enum.Parse(propertyType, value, true);
            }
            else if (propertyType.Name.Equals("TimeSpan"))
            {
                return TimeSpan.Parse(value, culture);
            }
            else if (propertyType == typeof(Guid))
            {
                return new Guid(value);
            }
            else if (propertyType == typeof(Uri))
            {
                return new Uri(value);
            }
            else
            {
                return Convert.ChangeType(value, propertyType, culture);
            }
        }

        // CSV format:
        // http://edoceo.com/utilitas/csv-file-format
        public static String XLSProtect(this String value)
        {
            if (value != null)
            {
                String text = value.Replace("<br>", "\n")
                                    .Replace("\r\n", " ")
                                    .Replace("\n", " ").Replace("\"", "\"\"");

                return @"""" + text + @"""";
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// Pour faire un lower case sur un tableau de string
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static string[] ToLower(this string[] array)
        {
            return array.Select(s => s.ToLower()).ToArray();
        }

#if !NETCOREAPP2_0
        /// <summary>
        /// Replace String in net461
        /// </summary>
        public static string Replace(this string originalString, string oldValue, string newValue, StringComparison comparisonType)
        {
            if (oldValue == null)
                throw new ArgumentNullException("oldValue");
            if (oldValue.Length == 0)
                throw new ArgumentException("String cannot be of zero length.", "oldValue");

            StringBuilder sb = null;

            int startIndex = 0;
            int foundIndex = originalString.IndexOf(oldValue, comparisonType);
            while (foundIndex != -1)
            {
                if (sb == null)
                    sb = new StringBuilder(originalString.Length + (newValue != null ? Math.Max(0, 5 * (newValue.Length - oldValue.Length)) : 0));
                sb.Append(originalString, startIndex, foundIndex - startIndex);
                sb.Append(newValue);

                startIndex = foundIndex + oldValue.Length;
                foundIndex = originalString.IndexOf(oldValue, startIndex, comparisonType);
            }

            if (startIndex == 0)
                return originalString;
#pragma warning disable S2259 
            sb.Append(originalString, startIndex, originalString.Length - startIndex);
#pragma warning restore S2259 
            return sb.ToString();
        }
#endif

        public static void AppendLineFormat(this StringBuilder sb, string format, params object[] args)
        {
            sb.AppendLine(string.Format(format, args));
        }

        /// <summary>
        /// http://stackoverflow.com/questions/472906/net-string-to-byte-array-c-sharp
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte[] ToByteArray(this string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        /// <summary>
        /// Annee10 => 10
        /// Annee => 1
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultValue (optional)"></param>
        /// <returns></returns>
        public static int GetOperand(this string str, int defaultValue = 1)
        {
            var stack = new Stack<char>();

            for (var i = str.Length - 1; i >= 0; i--)
            {
                if (!char.IsNumber(str[i]))
                {
                    break;
                }

                stack.Push(str[i]);
            }

            return stack.Any() ? Int32.Parse(new string(stack.ToArray())) : defaultValue;
        }

        /// <summary>
        /// Annee => Annee5
        /// </summary>
        /// <param name="str"></param>
        /// <param name="operand"></param>
        /// <returns></returns>
        public static string AddOperand(this string str, int operand)
        {
            return String.Format("{0}{1}", str, operand);
        }

        /// <summary>
        /// Annee10 => Annee
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string RemoveOperand(this string str)
        {
            return new string(str.TakeWhile(c => !Char.IsNumber(c)).ToArray());
        }

        /// <summary>
        /// Annee10 => Annee11
        /// Annee => Annee2
        /// </summary>
        /// <param name="str"></param>
        /// <param name="increment (optional)"></param>
        /// <returns></returns>
        public static string IncrementOperand(this string str, int increment = 1)
        {
            return String.Format("{0}{1}", str.RemoveOperand(), str.GetOperand() + increment);
        }

        /// <summary>
        /// Remove special characters to transform input into an valid file name
        /// Replaces spaces by dots
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToFileName(this string input)
        {
            return string.IsNullOrEmpty(input) ? input : Regex.Replace(input, @"[^\w. ]+", "").Replace(" ", ".");
        }
    }
}