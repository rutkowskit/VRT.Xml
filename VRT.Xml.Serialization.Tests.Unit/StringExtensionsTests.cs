using System;
using System.Text;
using VRT.Xml.Serialization.TestModels;
using Xunit;

namespace VRT.Xml.Serialization.Tests
{
    public class StringExtensionsTests
    {
        private const string TestXmlDocument = @"<Document xmlns=""http://test.pl""><Id>456</Id></Document>";
        private const string TestXmlDocumentWithDifferentRoot = @"<Document2 xmlns=""http://test.pl""><Id>456</Id></Document2>";
        private const string TestXmlDocumentWithDifferentNs = @"<Document xmlns=""http://test3.pl""><Id>456</Id></Document>";

        [Fact()]
        public void Deserialize_CorrectXml_CorrectDeserializedModelValues()
        {
            var result = TestXmlDocument.Deserialize<TestXmlDocument>();

            Assert.NotNull(result);
            Assert.Equal("456", result.Id);
        }

        [Fact()]
        public void Deserialize_XmlWithDifferentRootElementName_InvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                var _ = TestXmlDocumentWithDifferentRoot.Deserialize<TestXmlDocument>();
            });            
        }

        [Fact()]
        public void Deserialize_CorrectXml1250Encoding_CorrectDeserializedModelValues()
        {
            var dstEncoding = GetEncoding(1250);
            var xml = "<Document xmlns=\"http://test.pl\"><Id>ęąółŁÓóś</Id></Document>"
                .ChangeXmlEncoding(dstEncoding);         
            
            var result = xml.Deserialize<TestXmlDocument>(dstEncoding);

            Assert.NotNull(result);
            Assert.Equal(@"ęąółŁÓóś", result.Id);
        }

        [Fact()]
        public void Deserialize_CorrectXmlWithPolishEncodingWithAsciiEncoding_WrongObjectValues()
        {
            var dstEncoding = GetEncoding(1250);
            var xml = "<Document xmlns=\"http://test.pl\"><Id>ęąółŁÓóś</Id></Document>"
                .ChangeXmlEncoding(dstEncoding);

            var result = xml.Deserialize<TestXmlDocument>(Encoding.ASCII);

            Assert.NotNull(result);
            Assert.NotEqual(@"ęąółŁÓóś", result.Id);
        }

        private static Encoding GetEncoding(int number)
            => CodePagesEncodingProvider.Instance.GetEncoding(number);        
    }
}