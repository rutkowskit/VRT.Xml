using System.Xml;
using System.Xml.Linq;

namespace VRT.Xml.Xades.Extensions
{
    internal static class XElementExtensions
    {
        public static XmlElement ToXmlElement(this XElement el)
        {
            var doc = new XmlDocument();
            doc.Load(el.CreateReader());
            return doc.DocumentElement;
        }
    }
}
