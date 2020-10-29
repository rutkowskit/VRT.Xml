using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace VRT.Xml.Xades.Extensions
{
    public static class X509ExtensionExtensions
    {
        public static AsnEncodedData ToAsnEncodedData(this X509Extension ext)
        {
            return null == ext ? null : new AsnEncodedData(ext.Oid, ext.RawData);
        }
    }
}
