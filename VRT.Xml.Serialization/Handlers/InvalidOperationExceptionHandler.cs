using System;

namespace VRT.Xml.Serialization.Handlers
{
    /// <summary>
    /// Default serialization exception handler
    /// </summary>
    public sealed class ExceptionHandler : IExceptionHandler
    {
        ///<inheritdoc/>
        public bool Handle(Exception ex)
        {
            return false;
        }
    }
}
