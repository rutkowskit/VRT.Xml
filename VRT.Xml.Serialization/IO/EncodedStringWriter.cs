using System.IO;
using System.Text;

namespace VRT.Xml.Serialization.IO
{
    internal sealed class EncodedStringWriter : StringWriter
    {
        public EncodedStringWriter(Encoding encoding)
        {
            Encoding = encoding;
        }
        public override Encoding Encoding { get; }
    }
}
