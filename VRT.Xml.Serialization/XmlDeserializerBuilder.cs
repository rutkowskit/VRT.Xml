using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using VRT.Xml.Serialization.Handlers;
using VRT.Xml.Serialization.IO;

namespace VRT.Xml.Serialization
{
    public sealed class XmlDeserializerBuilder
    {
        public string DefaultDstNamespace { get; private set; }
        private readonly Dictionary<string, string> _nsOverrides;
        private bool _determineDstRootElement;
        private IExceptionHandler _exceptionHandler;

        public Encoding Encoding { get; private set; }
        public string RootElementName { get; private set; }

        public XmlDeserializerBuilder()
        {
            DefaultDstNamespace = string.Empty;
            _nsOverrides = new Dictionary<string, string>();
            _exceptionHandler = new ExceptionHandler();
            _determineDstRootElement = true;
        }

        #region Fluent methods
        public XmlDeserializerBuilder WithDefaultNamespaceOverride(string defaultDstNamespace)
        {
            DefaultDstNamespace = defaultDstNamespace;
            return this;
        }

        public XmlDeserializerBuilder WithEncoding(Encoding encoding)
        {
            Encoding = encoding;
            return this;
        }

        public XmlDeserializerBuilder AddNamespaceOverride(string srcNamespaceUri, string dstNamespaceUri)
        {
            if (null == srcNamespaceUri || null==dstNamespaceUri)
                return this;
            _nsOverrides[srcNamespaceUri] = dstNamespaceUri;
            return this;
        }

        public XmlDeserializerBuilder WithDstRootElementName(string rootElementName)
        {
            if (string.IsNullOrWhiteSpace(rootElementName))
                return this;
            RootElementName = rootElementName;
            return this;
        }

        public XmlDeserializerBuilder WithAutoDestinationRootElement(bool determineDstRootElement)
        {
            _determineDstRootElement = determineDstRootElement;
            return this;
        }
        public XmlDeserializerBuilder WithExceptionHandler(IExceptionHandler handler)
        {
            _exceptionHandler = handler;
            return this;
        }
        #endregion

        public TModel Deserialize<TModel>(string xml)
        {
            if (_determineDstRootElement)
            {
                SetupDstRootElement<TModel>();
                var (name, ns) = xml.GetRootNamespace();
                AddNamespaceOverride(ns, DefaultDstNamespace);
                if (RootElementName!=name)
                    RootElementName = name;
            }

            using (var stream = xml.AsMemoryStream(Encoding ?? Encoding.UTF8))
            using (var reader = BuildReader(stream))
            {
                try
                {
                    return (TModel)CreateDeserializer<TModel>()
                        .Deserialize(reader);
                }
                catch (InvalidOperationException iox)
                {
                    if (null == _exceptionHandler || !_exceptionHandler.Handle(iox))
                        throw;
                    return default;
                }
            }
        }

        private void SetupDstRootElement<TModel>()
        {   
            try
            { 
                var type = typeof(TModel);
                var _ = SetDstRootForXmlRootAttribute(type)
                    || SetDstRootForXmlTypeAttribute(type);
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch
            {
            }
        }

        private bool SetDstRootForXmlRootAttribute(MemberInfo type)
        {
            var xmlRootAttribute = (XmlRootAttribute)Attribute.GetCustomAttribute(
                type, typeof(XmlRootAttribute)
            );
            if (null == xmlRootAttribute) return false;

            WithDstRootElementName(xmlRootAttribute.ElementName ?? type.Name)
                .WithDefaultNamespaceOverride(xmlRootAttribute.Namespace ?? "");

            return true;
        }
        
        private bool SetDstRootForXmlTypeAttribute(MemberInfo type)
        {
            var xmlAttribute = (XmlTypeAttribute)Attribute.GetCustomAttribute(
                type, typeof(XmlTypeAttribute)
            );
            WithDefaultNamespaceOverride(xmlAttribute?.Namespace ?? "")
                .WithDstRootElementName(type.Name);
            return true;
        }
        
        private XmlSerializer CreateDeserializer<TModel>()
        {
            XmlRootAttribute rootAttribute = null;
            if (!string.IsNullOrWhiteSpace(RootElementName))
            {
                rootAttribute = new XmlRootAttribute
                {
                    ElementName = RootElementName,
                    Namespace = DefaultDstNamespace,
                    IsNullable = true
                };
            }

            var deserializer = rootAttribute == null
                ? CachingXmlSerializerFactory.Create<TModel>()
                : CachingXmlSerializerFactory.Create<TModel>(rootAttribute);
            return deserializer;
        }

        private XmlReader BuildReader(Stream stream) => 
            BuildReader(stream, Encoding ?? Encoding.UTF8);
        
        private XmlReader BuildReader(Stream stream, Encoding encoding){
            
            var reader = new StreamReader(stream, encoding, false);
            return BuildReader(reader);
        }

        private XmlReader BuildReader(TextReader reader)
        {
            if(DefaultDstNamespace==null && _nsOverrides.Count==0)
                return XmlReader.Create(reader);

            var result = new XmlReaderNoNamespaces(reader);

            if (null != DefaultDstNamespace && !_nsOverrides.ContainsValue(DefaultDstNamespace))
                result.OverrideNamespace(DefaultDstNamespace);

            foreach (var nsOverride in _nsOverrides)
            {
                result.AddNamespaceUriOverride(nsOverride.Key, nsOverride.Value);
            }
            return result;
        }
    }
}
