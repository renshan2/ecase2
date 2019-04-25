using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Treasury.ECM.eCase.SusDeb.DOI.Extensions
{
    public static class StringExtensions
    {
        // Code from http://stackoverflow.com/questions/2920744/url-slugify-alrogithm-in-c
        public static string GenerateSlug(this string phrase)
        {
            string str = phrase.RemoveAccent().ToLower();
            str = Regex.Replace(str, @"[^a-z0-9\s-]", "_"); // Replace all non-alphanumeric characters with underscore
            if (str[0] == '_')
                str = 'a' + str; // prepend an alphanumeric to the string so that it is a valid url
            str = Regex.Replace(str, @"\s+", " ").Trim(); // convert multiple spaces into one space   
            str = str.Substring(0, str.Length <= 45 ? str.Length : 45).Trim(); // cut and trim 
            str = Regex.Replace(str, @"\s", "-"); // hyphens   
            return str;
        }

        // Code from http://stackoverflow.com/questions/2920744/url-slugify-alrogithm-in-c
        public static string RemoveAccent(this string txt)
        {
            byte[] bytes = System.Text.Encoding.GetEncoding("Cyrillic").GetBytes(txt);
            return System.Text.Encoding.ASCII.GetString(bytes);
        }
    }
}
