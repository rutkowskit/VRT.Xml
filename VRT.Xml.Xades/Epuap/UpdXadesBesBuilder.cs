using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Xades;
using VRT.Xml.Xades.Extensions;
using VRT.Xml.Xades.Helpers;
using VRT.Xml.Xades.Services;
using VRT.Xml.Xades.StaticSets;

namespace VRT.Xml.Xades.Epuap
{
    public class XadesSignedUpdBuilder
    {

        private ICertyficateService _certService;
        private readonly string _updXml;
        private bool _qualifiedCertsOnly;

        private XadesSignedUpdBuilder(string updXml, ICertyficateService certService)
        {
            _certService = certService; 
            _updXml = updXml;
        }
        
        public X509Certificate2 UsedCertyficate { get; private set; }

        public static XadesSignedUpdBuilder Create(string updXml, ICertyficateService certService)
        {
            if(string.IsNullOrWhiteSpace(updXml))
                throw new ArgumentNullException(nameof(updXml));
            return new XadesSignedUpdBuilder(updXml, certService);
        }
        
        public XmlDocument Build()
        {
            var references = GetTemplateReferences();
            var cert = _certService.GetCertyficate();
            AssertCert(cert);

            UsedCertyficate = cert;
            var xml = GetUpdXml();
            var doc = new XmlDocument { PreserveWhitespace = true };
            doc.LoadXml(xml);

            var signedXml = new XadesSignedXml(doc)
                {
                    NoRefreshComputedDigests = true,
                    AddXadesNamespace = false
                }
                .SetSigningCertyficate(references.SignatureId, cert)
                .DodajReferencje(references)
                .AddKeyInfo(cert)
                .AddXadesObject(cert);

            signedXml.ComputeSignature();

            var nodeToAdd = doc.ImportNode(signedXml.GetXml(), true);
            doc.DocumentElement?.AppendChild(nodeToAdd);
            return doc;
        }

        private void AssertCert(X509Certificate2 cert)
        {
            if(null==cert)
                throw new ArgumentNullException(nameof(cert));
            if (_qualifiedCertsOnly && !Qca.IsQca(cert))
                throw new ArgumentOutOfRangeException(nameof(cert),Messages.CertyficateNotQualified);
        }

        public string BuildToXml()
        {
            var doc = Build();
            using (var stream = new Utf8StringWriter())
            {
                doc.Save(stream);
                stream.Flush();
                return stream.ToString();
            }
        }
        private ReferenceLoader GetTemplateReferences()
        {
            var result = ReferenceLoader.LoadFromXml(_updXml);
            if (null == result)
            {
                throw new ApplicationException("Brak referencji bazowych w sygnaturze UPD");
            }
            return result;
        }

        public XadesSignedUpdBuilder WithCertyficateService(ICertyficateService service)
        {
            _certService = service ?? _certService;
            return this;
        }

        public XadesSignedUpdBuilder RequireQualifiedCertyficate(bool require)
        {
            _qualifiedCertsOnly = require;
            return this;
        }

        public XadesSignedUpdBuilder AddQca(string qcaOid)
        {
            Qca.AddQca(qcaOid);
            return this;
        }

        private string GetUpdXml()
        {
            var doc = XDocument.Parse(_updXml);
            doc.Descendants()
                .Where(d => d.Name.LocalName == "Signature")
                .Remove();

            var dataOdb = doc.Descendants()
                .FirstOrDefault(d => d.Name.LocalName == "UPD")
                ?.Descendants()
                .FirstOrDefault(d => d.Name.LocalName == "DataOdbioru");
            if (null == dataOdb)
                throw new NullReferenceException("Brak tag-u dataodbioru w xml-u");
            dataOdb.Value = DateTime.Now.ToString("yyyy-MM-dd");

            return doc.ToString(SaveOptions.None);
        }
    }
}
