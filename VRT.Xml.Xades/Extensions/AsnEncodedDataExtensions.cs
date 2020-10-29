using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace VRT.Xml.Xades.Extensions
{
    public static class AsnEncodedDataExtensions
    {
        public static IEnumerable<string> GetOids(this AsnEncodedData ext)
        {
            if(null==ext) yield break;
            var txt = ext.Format(false);
            var matches = Regex.Matches(txt, @"\b(?<oid>(?:\d{1,}\.?){8,})\b", RegexOptions.Singleline);
            foreach (Match match in matches)
            {
                yield return match.Groups["oid"].Value;
            }
        }
    }
}
