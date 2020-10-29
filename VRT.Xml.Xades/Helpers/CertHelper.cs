using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace VRT.Xml.Xades.Helpers
{
    public static class CertHelper
    {
        public static string GetSerialNumberAsDecimalString(this X509Certificate2 certificate)
        {
            if (null == certificate?.SerialNumber) return string.Empty;

            var dec = new List<int> { 0 };

            foreach (var c in certificate.SerialNumber)
            {
                var carry = Convert.ToInt32(c.ToString(), 16);
                for (var i = 0; i < dec.Count; ++i)
                {
                    var val = dec[i] * 16 + carry;
                    dec[i] = val % 10;
                    carry = val / 10;
                }
                while (carry > 0)
                {
                    dec.Add(carry % 10);
                    carry /= 10;
                }
            }

            var chars = dec.Select(d => (char)('0' + d));
            var cArr = chars.Reverse().ToArray();
            return new string(cArr);
        }
    }
}
