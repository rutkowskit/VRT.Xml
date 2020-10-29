using System;
using System.Text;
using VRT.Xml.Serialization.Fakes;
using VRT.Xml.Serialization.TestModels;
using Xunit;

namespace VRT.Xml.Serialization
{
    public class XmlDeserializerBuilderTests
    {
        [Fact()]
        public void Deserialize_SerializedXml_EqualModelValues()
        {
            var expected = new TestXmlDocument()
            {
                Id = "123",
                Signature = "321"
            };
            var xml = expected.Serialize();

            var result = CreateSut()
                .Deserialize<TestXmlDocument>(xml);

            Assert.Equal(expected.Id,result.Id);
            Assert.Equal(expected.Signature,result.Signature);
        }

        [Fact()]
        public void Deserialize_FromXmlToPocoModel_CorrectPocoObject()
        {
            var xml = new TestXmlDocument()
                {
                    Id = "123",
                    Signature = "321"
                }.Serialize();

            var result = CreateSut()
                .Deserialize<TestDocumentModel>(xml);

            Assert.Equal("321", result.Signature);
        }
        [Fact()]
        public void Deserialize_FromPocoToXmlModel_CorrectXmlObject()
        {
            var xml = new TestDocumentModel()
            {
                Signature = "321"
            }.Serialize();

            var result = CreateSut()
                .Deserialize<TestXmlDocument>(xml);

            Assert.Equal("321", result.Signature);
        }

        [Fact()]
        public void Deserialize_FromPocoToPocoModel_CorrectXmlObject()
        {
            var xml = new TestDocumentModel()
            {
                Signature = "321"
            }.Serialize();

            var result = CreateSut()
                .Deserialize<TestDocumentModel>(xml);

            Assert.Equal("321", result.Signature);
        }

        [Fact()]
        public void Deserialize_FromDamagedXmlWithoutExceptionHandler_InvalidOperationException()
        {
            var xml = @"<bad xml <garbage";

            Assert.Throws<InvalidOperationException>(() =>
            {
                var _ = CreateSut()
                    .Deserialize<TestDocumentModel>(xml);
            });
        }

        [Fact()]
        public void Deserialize_FromDamagedXmlWithHandlingExceptionHandler_Null()
        {
            var xml = @"<bad xml <garbage";
            var handler = new FakeExceptionHandler(true);

            var result = CreateSut()
                .WithExceptionHandler(handler)
                .Deserialize<TestDocumentModel>(xml);
            Assert.Null(result);
            Assert.Equal(1, handler.ExceptionHandledCount);
        }

        private XmlDeserializerBuilder CreateSut()
        {
            return new XmlDeserializerBuilder()
                .WithEncoding(Encoding.UTF8);
        }        
    }
}