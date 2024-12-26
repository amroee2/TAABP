namespace TAABP.Application.DTOs
{
    public class ReservationDto
    {
        public int ReservationId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int RoomId { get; set; }
        public string UserId { get; set; }
    }
}
