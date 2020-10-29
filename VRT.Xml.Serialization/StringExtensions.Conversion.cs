using System.Text;
using System.Xml;
using VRT.Xml.Serialization.IO;

namespace VRT.Xml.Serialization
{
    static partial class StringExtensions
    {
        /// <summary>
        /// Changes xml encoding
        /// </summary>
        /// <param name="xml">Xml text</param>
        /// <param name="dstEncoding">Destination encoding</param>
        /// <returns>Xml with changed encoding</returns>
        public static string ChangeXmlEncoding(this string xml, Encoding dstEncoding)
        {
            if (string.IsNullOrWhiteSpace(xml)) return "";
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            var converted = ChangeXmlEncoding(xmlDoc, dstEncoding.WebName);
            using (var stringWriter = new EncodedStringWriter(dstEncoding))
            using (var xmlTextWriter = XmlWriter.Create(stringWriter))
            {
                converted.WriteTo(xmlTextWriter);
                xmlTextWriter.Flush();
                return stringWriter.GetStringBuilder().ToString();
            }
        }
        private static XmlDocument ChangeXmlEncoding(XmlDocument xmlDoc, string newEncoding)
        {            
            if (null == xmlDoc) return null;
            if (xmlDoc.FirstChild.NodeType != XmlNodeType.XmlDeclaration) return xmlDoc;
            var xmlDeclaration = (XmlDeclaration)xmlDoc.FirstChild;
            xmlDeclaration.Encoding = newEncoding;
            return xmlDoc;
        }
    }
}
