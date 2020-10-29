using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using Microsoft.Xades;
using VRT.Xml.Xades.Helpers;

namespace VRT.Xml.Xades.Extensions
{
    internal static class XadesObjectExtensions
    {
        public static XadesObject AddCertificate(this XadesObject xadesObject, X509Certificate2 cert)
        {
            if (null == xadesObject || null == cert) return xadesObject;
            var xadesCert = new Cert
            {
                IssuerSerial =
                {
                    X509IssuerName = cert.IssuerName.Name,
                    X509SerialNumber = cert.GetSerialNumberAsDecimalString()
                }
            };
            xadesCert.CertDigest.DigestMethod.Algorithm = SignedXml.XmlDsigSHA1Url;
            using (var hash = new SHA1CryptoServiceProvider())
            {
                xadesCert.CertDigest.DigestValue = hash.ComputeHash(cert.GetRawCertData());
            }
            xadesObject.QualifyingProperties.SignedProperties
                .SignedSignatureProperties
                .SigningCertificate
                .CertCollection.Add(xadesCert);
            return xadesObject;
        }
    }
}
