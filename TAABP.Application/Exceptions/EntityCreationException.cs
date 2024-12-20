namespace TAABP.Application.Exceptions
{
    public class EntityCreationException : Exception
    {
        public EntityCreationException(string message) : base(message)
        {
        }

        public EntityCreationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public EntityCreationException()
        {
        }
    }
}
