using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Xml.Linq;
using VRT.Xml.Xades.Extensions;

namespace VRT.Xml.Xades
{
    internal class ReferenceLoader
    {
        public ICollection<Reference> SignatureReferences { get; } = new HashSet<Reference>();
        public string SignatureId { get; private set; }
        private ReferenceLoader()
        {
            
        }

        private void AddReference(Reference reference)
        {
            if (null == reference) return;
            SignatureReferences.Add(reference);
        }
        public static ReferenceLoader LoadFromXml(string xml)
        {
            var result = new ReferenceLoader();
            if (string.IsNullOrWhiteSpace(xml)) return result;

            var signature = XDocument.Parse(xml)
                .Descendants()
                .FirstOrDefault(d => d.Name.LocalName == "Signature");

            var refs = signature?.Descendants()
                .Where(d => d.Name.LocalName == "Reference")
                // pomijamy referencje do "siebie samego"
                //.Where(d => d.Attributes().Any(a => a.Name.LocalName == "URI" && !string.IsNullOrWhiteSpace(a.Value)))
                .Where(d => d.Attributes().Any(a => a.Name.LocalName == "Id" && !string.IsNullOrWhiteSpace(a.Value)))
                .ToList();

            result.SignatureId = signature?.Attributes()
                .FirstOrDefault(a => a.Name.LocalName == "Id")
                ?.Value;
            
            if (null == refs || refs.Count == 0) return result;
            foreach (var r in refs)
            {
                var rRes = new Reference();
                rRes.LoadXml(r.ToXmlElement());
                result.AddReference(rRes);
            }
            return result;
        }
    }
}
