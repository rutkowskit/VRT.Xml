using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace VRT.Xml.Serialization.TestModels
{
    [ExcludeFromCodeCoverage]
    [Serializable()]
    [System.Xml.Serialization.XmlType(Namespace="http://test.pl")]
    [System.Xml.Serialization.XmlRoot("BadDocument", Namespace="http://testbad.pl", IsNullable=false)]
    public class BadTestDocument
    {
        public IEnumerable<int> Id { get; set; }
    }
}