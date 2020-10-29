using System.IO;
using System.Text;
using System.Xml;
namespace VRT.Xml.Serialization
{
    public static partial class StringExtensions
    {        

        public static TModel Deserialize<TModel>(this string xml)
        {
            return xml.Deserialize<TModel>(Encoding.UTF8);
        }

        public static TModel Deserialize<TModel>(this string xml, Encoding encoding)
        {
            return xml.Deserialize<TModel>(encoding, null, null);
        }

        public static TModel Deserialize<TModel>(this string xml, Encoding encoding, 
            string rootElement, string rootNamespace)
        {
            return new XmlDeserializerBuilder()
                .WithAutoDestinationRootElement(false)
                .WithEncoding(encoding)
                .WithDstRootElementName(rootElement)
                .WithDefaultNamespaceOverride(rootNamespace)
                .Deserialize<TModel>(xml);
        }

        internal static Stream AsMemoryStream(this string stringText, Encoding encoding)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream, encoding);
            writer.Write(stringText);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
        internal static (string name, string @namespace) GetRootNamespace(this string xml)
        {
            try
            {
                var doc = new XmlDocument();
                doc.LoadXml(xml);
                return (doc.DocumentElement?.LocalName ??"", doc.DocumentElement?.NamespaceURI ?? "");
            }
            catch
            {
                return ("","");
            }
        }
    }
}
