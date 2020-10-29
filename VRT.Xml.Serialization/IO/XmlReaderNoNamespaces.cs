using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace VRT.Xml.Serialization.IO
{
    /// <summary>
    /// XmlTextReader with abitlity to ignore or change xml namespaces
    /// </summary>
    public sealed class XmlReaderNoNamespaces : XmlTextReader
    {
        private string _namespace;
        private readonly Dictionary<string,string> _nsOverrides = new Dictionary<string, string>();

        ///<inheritdoc/>
        public XmlReaderNoNamespaces(Stream stream) : base(stream)
        {
        }
        ///<inheritdoc/>
        public XmlReaderNoNamespaces(TextReader stream) : base(stream)
        {
        }

        /// <summary>
        /// Sets default namespace to override during read
        /// </summary>
        /// <param name="namespaceUri">Namespace uri</param>
        /// <returns>Object instance</returns>
        public XmlReaderNoNamespaces OverrideNamespace(string namespaceUri)
        {            
            // namespace must be intern string otherwise it will not work correctly
            _namespace = namespaceUri==null ? null : string.Intern(namespaceUri);
            return this;
        }

        /// <summary>
        /// Adds additional internal namespace uri to override during read
        /// </summary>
        /// <param name="srcNamespaceUri">Namespace URI in source xml</param>
        /// <param name="dstNamespaceUri">Namespace URI in destination model</param>
        /// <returns>Object instance</returns>
        public XmlReaderNoNamespaces AddNamespaceUriOverride(string srcNamespaceUri, string dstNamespaceUri)
        {
            if (srcNamespaceUri == null || dstNamespaceUri == null)
                return this;
            _nsOverrides[srcNamespaceUri] = string.Intern(dstNamespaceUri);
            return this;
        }

        ///<inheritdoc/>
        public override string Name => LocalName; // name without namespace        

        ///<inheritdoc/>
        public override string NamespaceURI
        {
            get
            {
                var baseUri = base.NamespaceURI;
                return _nsOverrides.TryGetValue(baseUri, out var ns)
                    ? ns
                    : _namespace ?? baseUri;
            }
        }

        ///<inheritdoc/>
        public override string Prefix => string.Empty;
    }
}
