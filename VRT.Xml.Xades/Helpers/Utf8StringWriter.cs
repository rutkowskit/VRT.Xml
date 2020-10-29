using System.IO;
using System.Text;

namespace VRT.Xml.Xades.Helpers
{
    internal class Utf8StringWriter : StringWriter
    {
        public Utf8StringWriter() : this(Encoding.UTF8)
        {
            
        }
        public Utf8StringWriter(Encoding encoding)
        {
            Encoding = encoding;
        }
        public override Encoding Encoding { get; }
    }
}
