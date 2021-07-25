using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HackMD_ImgDownloader
{
    public static class RegexUtil
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="pattern"></param>
        /// <param name="groupName"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static string RegexMatch(
            string input,
            string pattern,
            string groupName = "valueName",
            RegexOptions options = RegexOptions.IgnoreCase | RegexOptions.Singleline
            )
        {
            string strMatch = "";
            Regex reg   = new Regex(pattern, options);
            Match match = reg.Match(input);
            if (   match        != null
                && match.Groups != null
                && match.Groups.Count > 0
                )
            {
                Group group = match.Groups[groupName];
                if (group != null)
                {
                    strMatch = group.Value;
                }
            }
            return strMatch;
        }

        public static List<string> RegexMatches(
            string input,
            string pattern,
            string groupName = "valueName",
            RegexOptions options = RegexOptions.IgnoreCase | RegexOptions.Singleline
            )
        {
            Regex reg   = new Regex(pattern, options);
            var matches = reg.Matches(input);
            List<string> lst = new List<string>();
            foreach (Match match in matches)
            {
                string strMatch = "";
                if (   match        != null
                    && match.Groups != null
                    && match.Groups.Count > 0
                    )
                {
                    Group group = match.Groups[groupName];
                    if (group != null)
                    {
                        strMatch = group.Value;
                    }
                }
                lst.Add(strMatch);
            }
            return lst;
        }

        /// <summary>
        /// よく使うタグで囲まれた値を取得する正規表現パターン。
        /// </summary>
        /// <param name="startTag"></param>
        /// <param name="endTag"></param>
        /// <param name="valueName"></param>
        /// <returns></returns>
        public static string RegPattern_Tag(
            string startTag,
            string endTag,
            string valueName = "valueName"
            )
        {
            StringBuilder sbr = new StringBuilder();
            sbr
                .Append(startTag)
                .Append(@"(?<")
                .Append(valueName)
                .Append(@">(?:(?!")
                .Append(endTag)
                .Append(@").)*)")
                .Append(endTag);
            return sbr.ToString();
        }

    }
}
