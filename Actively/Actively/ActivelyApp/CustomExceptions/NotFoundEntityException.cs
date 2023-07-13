using System.Runtime.Serialization;

namespace ActivelyApp.CustomExceptions
{
    [Serializable]
    internal class NotFoundEntityException : Exception
    {
        public NotFoundEntityException()
        {
        }

        public NotFoundEntityException(string? message) : base(message)
        {
        }

        public NotFoundEntityException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected NotFoundEntityException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}