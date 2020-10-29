using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using VRT.Xml.Xades.Extensions;

namespace VRT.Xml.Xades.StaticSets
{
    /// <summary>
    /// Qualification Certification Accreditation
    /// </summary>
    public static class Qca
    {
        private static readonly ICollection<string> Collection = new HashSet<string>(new[]
        {
            "1.2.616.1.113527.2.4.1.1",
            "1.2.616.1.113560.10.1.1.0",
            "1.2.616.1.113725.0.0.0.1",
            "1.2.616.1.113571.1.1",
            "1.2.616.1.113571.1.2",
            "1.2.616.1.113571.1.3",
            "1.2.616.1.113571.1.4",
            "1.3.6.1.4.1.7999.2.300.10.1.1.0",
            "1.3.6.1.4.1.7999.2.300.10.1.1.1",
            "1.3.6.1.4.1.23554.1.1",
            "1.2.616.1.113736.1.1.2",
            "1.2.616.1.113681.1.1.10.1.1.2",
            "1.3.6.1.4.1.10214.99.1.1.1.4",
            "1.3.6.1.4.1.23554.2.1",
            "1.2.616.1.113791.1.2.1",
            "1.2.616.1.113791.1.2.2",
            "1.2.616.1.113527.2.4.1.11",
            "1.2.616.1.113527.2.4.1.12.1",
            "1.2.616.1.113527.2.4.1.12.2",
            "1.2.616.1.113527.2.4.1.13.1",
            "1.2.616.1.113527.2.4.1.13.2"
        });

        private static readonly ICollection<string> AdditionalQca = new HashSet<string>();

        public static bool IsQca(string identyfier)
        {
            return IsQca(identyfier, true);
        }
        public static bool IsQca(string identyfier, bool checkAdditional)
        {
            if (string.IsNullOrWhiteSpace(identyfier)) return false;
            return Collection.Contains(identyfier) || (checkAdditional && AdditionalQca.Contains(identyfier));
        }
        public static bool IsQca(X509Certificate2 cert)
        {
            if (null == cert) return false;

            foreach (var certExtension in cert.Extensions)
            {
                if (certExtension.Oid.Value != "2.5.29.32") continue;

                var asndata = certExtension.ToAsnEncodedData()
                    .GetOids()
                    .FirstOrDefault();
                if (null == asndata) continue;
                return IsQca(asndata);
            }
            return false;
        }
        public static void AddQca(string identyfier)
        {
            AdditionalQca.Add(identyfier);
        }

        public static void ResetQcaList()
        {
            AdditionalQca.Clear();
        }
    }
}
