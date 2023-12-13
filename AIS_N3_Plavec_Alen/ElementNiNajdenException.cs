using System.Runtime.Serialization;

namespace WebApi
{
    [Serializable]
    internal class ElementNiNajdenException : Exception
    {
        public ElementNiNajdenException()
        {
        }

        public ElementNiNajdenException(string? message) : base(message)
        {
        }

        public ElementNiNajdenException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected ElementNiNajdenException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}