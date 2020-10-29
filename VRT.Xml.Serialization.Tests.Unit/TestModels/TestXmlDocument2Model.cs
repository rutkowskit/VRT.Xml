using System.Diagnostics.CodeAnalysis;

namespace VRT.Xml.Serialization.TestModels
{
    [ExcludeFromCodeCoverage]
    [System.Serializable()]
    [System.Xml.Serialization.XmlType(Namespace="http://test2.pl")]
    [System.Xml.Serialization.XmlRoot("Document2", Namespace="http://test2.pl", IsNullable=false)]
    public class TestXmlDocument2Model
    {
        public string Id { get; set; }
        public string Signature { get; set; }
    }
}