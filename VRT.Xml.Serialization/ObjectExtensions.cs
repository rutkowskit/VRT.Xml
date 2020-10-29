using System;
using System.Text;
using VRT.Xml.Serialization.Handlers;
using VRT.Xml.Serialization.IO;

namespace VRT.Xml.Serialization
{
    /// <summary>
    /// Extends reference type object with ability to serialize to xml format
    /// </summary>
    public static class ObjectExtensions
    {
        public static string Serialize<TModel>(this TModel model)
            where TModel : class
        {
            return Serialize(model,Encoding.UTF8, new ExceptionHandler());
        }
        public static string Serialize<TModel>(this TModel model,Encoding encoding,
            IExceptionHandler exceptionHandler)
            where TModel:class
        {
            if (null == model) return "";

        
            using (var stringWriter = new EncodedStringWriter(encoding))
            {
                try
                {
                    CachingXmlSerializerFactory
                        .Create<TModel>()
                        .Serialize(stringWriter, model);
                    return stringWriter.ToString();
                }
                catch (InvalidOperationException ex)
                {
                    if (null == exceptionHandler || !exceptionHandler.Handle(ex))
                        throw;
                    return null;
                }
            }
        }
    }
}
