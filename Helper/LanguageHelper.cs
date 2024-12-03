using System.Text.RegularExpressions;

namespace Lemoo_pos.Helper
{
    public class LanguageHelper
    {
        public static string RemoveVietnameseTones(string str)
        {
            // Replace accented characters with unaccented equivalents
            str = Regex.Replace(str, "[àáạảãâầấậẩẫăằắặẳẵ]", "a");
            str = Regex.Replace(str, "[èéẹẻẽêềếệểễ]", "e");
            str = Regex.Replace(str, "[ìíịỉĩ]", "i");
            str = Regex.Replace(str, "[òóọỏõôồốộổỗơờớợởỡ]", "o");
            str = Regex.Replace(str, "[ùúụủũưừứựửữ]", "u");
            str = Regex.Replace(str, "[ỳýỵỷỹ]", "y");
            str = Regex.Replace(str, "[đ]", "d");

            str = Regex.Replace(str, "[ÀÁẠẢÃÂẦẤẬẨẪĂẰẮẶẲẴ]", "A");
            str = Regex.Replace(str, "[ÈÉẸẺẼÊỀẾỆỂỄ]", "E");
            str = Regex.Replace(str, "[ÌÍỊỈĨ]", "I");
            str = Regex.Replace(str, "[ÒÓỌỎÕÔỒỐỘỔỖƠỜỚỢỞỠ]", "O");
            str = Regex.Replace(str, "[ÙÚỤỦŨƯỪỨỰỬỮ]", "U");
            str = Regex.Replace(str, "[ỲÝỴỶỸ]", "Y");
            str = Regex.Replace(str, "[Đ]", "D");

            // Remove combining accent characters
            str = Regex.Replace(str, "[\u0300\u0301\u0303\u0309\u0323]", ""); // ̀ ́ ̃ ̉ ̣
            str = Regex.Replace(str, "[\u02C6\u0306\u031B]", ""); // ˆ ̆ ̛

            // Remove extra spaces
            str = Regex.Replace(str, @"\s+", " ").Trim();

            // Remove punctuations and special characters
            str = Regex.Replace(str, @"[!@%\^*\(\)\+=\<\>\?\/,.:;'""&\#\[\]~$_`\-\{\}\|\\]", " ");

            return str;
        }
    }
}
