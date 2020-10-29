using System.Security.Cryptography.X509Certificates;

namespace VRT.Xml.Xades.Services
{
    public interface ICertyficateService
    {
        X509Certificate2 GetCertyficate();
    }
}
