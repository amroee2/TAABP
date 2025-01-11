namespace TAABP.Application.Exceptions
{
    public class RoomAlreadyBookedException : Exception
    {
        public RoomAlreadyBookedException(string message) : base(message)
        {
        }

        public RoomAlreadyBookedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public RoomAlreadyBookedException()
        {
        }
    }
}
