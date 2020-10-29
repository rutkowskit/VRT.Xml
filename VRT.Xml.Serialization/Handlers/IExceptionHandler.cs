using System;

namespace VRT.Xml.Serialization.Handlers
{
    /// <summary>
    /// Serialization exception handler
    /// </summary>
    public interface IExceptionHandler
    {
        /// <summary>
        /// Exception handler
        /// </summary>
        /// <param name="ex">Exception to be handled</param>
        /// <returns>Should return true if exception was handled, otherwise false</returns>
        bool Handle(Exception ex);
    }
}