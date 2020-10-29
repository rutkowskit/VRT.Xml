using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using VRT.Xml.Serialization.Fakes;
using VRT.Xml.Serialization.TestModels;
using Xunit;

namespace VRT.Xml.Serialization
{
    [ExcludeFromCodeCoverage]
    public class ObjectExtensionsTests
    {
        [Fact()]
        public void Serialize_DocumentNull_EmptyString()
        {
            var xml = ((TestXmlDocument) null).Serialize();

            Assert.Equal("", xml);
        }

        [Fact()]
        public void Serialize_CorrectDocument_CorrectXml()
        {
            var xml = new TestXmlDocument()
            {
                Id = "111",
                Signature = "XXY"
            }.Serialize();

            Assert.False(string.IsNullOrWhiteSpace(xml),"string.IsNullOrWhiteSpace(xml)");
        }

        [Fact()]
        public void Serialize_BadDocumentModel_InvalidOperationException()
        {

            Assert.Throws<InvalidOperationException>(() =>
            {
                var _ = new BadTestDocument()
                {
                    Id = new [] {123}
                }.Serialize();
            });
        }
        [Fact()]
        public void Serialize_BadDocumentModelAndNoHandlingExceptionHandler_InvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                var handler = new FakeExceptionHandler(false);
                var _ = new BadTestDocument()
                {
                    Id = new [] {123}
                }.Serialize(Encoding.UTF8, handler);
            });
        }
        [Fact()]
        public void Serialize_BadDocumentModelAndNullExceptionHandler_InvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                var _ = new BadTestDocument()
                {
                    Id = new [] {123}
                }.Serialize(Encoding.UTF8, null);
            });
        }
        [Fact()]
        public void Serialize_BadDocumentModelAndHandlingExceptionHandler_NullString()
        {
            var handler = new FakeExceptionHandler(true);
            var result = new BadTestDocument()
                {
                    Id = new [] {123}
                }.Serialize(Encoding.UTF8, handler);
            Assert.Null(result);
            Assert.Equal(1,handler.ExceptionHandledCount);
        }
    }
}