namespace VRT.Xml.Serialization.TestModels
{
    [System.Serializable()]
    [System.Xml.Serialization.XmlType(Namespace="http://test.pl")]
    [System.Xml.Serialization.XmlRoot("Document", Namespace="http://test.pl", IsNullable=false)]
    public class TestXmlDocument
    {
        public string Id { get; set; }
        public string Signature { get; set; }
    }
}