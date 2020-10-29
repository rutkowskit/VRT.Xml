using System;
using System.Collections.Generic;
using System.Security.Cryptography.Xml;
using System.Xml;

namespace VRT.Xml.Xades.Transforms
{
    internal class XPathTransformBuilder<TContext>
    {
        private readonly Dictionary<string, string> _namespaces = new Dictionary<string, string>();
        private readonly TContext _context;
        private readonly Action<Transform> _onDoneAction;
        private string _xpath;

        public XPathTransformBuilder(TContext context, Action<Transform> onDoneAction)
        {
            _context = context;
            _onDoneAction = onDoneAction;
        }
        public XPathTransformBuilder<TContext> WithXpath(string xpath)
        {
            _xpath = xpath;
            return this;
        }
        public XPathTransformBuilder<TContext> AddNamespace(string alias, string uri)
        {
            _namespaces[alias] = uri;
            return this;
        }
        public TContext Done()
        {
            var result = CreateXPathTransform(_xpath, _namespaces);
            _onDoneAction?.Invoke(result);
            return _context;
        }

        private static XmlDsigXPathTransform CreateXPathTransform(string xpath, IDictionary<string, string> xpathNamespaces)
        {
            var doc = new XmlDocument();
            var xpathElem = doc.CreateElement("XPath");
            if (null != xpathNamespaces)
            {
                foreach (var n in xpathNamespaces)
                {
                    xpathElem.SetAttribute(n.Key, n.Value);
                }
            }
            xpathElem.InnerText = xpath;

            var xform = new XmlDsigXPathTransform();
            var nodes = xpathElem.SelectNodes(".");
            if (null == nodes) return null;
            xform.LoadInnerXml(nodes);
            return xform;
        }
    }
}
