namespace backend.Models
{
    public class PatientQueue
    {
       public int Id { get; set; }

        public int PatientId { get; set; }

        public int RoomId { get; set; }

        public int QueueNumber { get; set; }

        public int PriorityLevel { get; set; } = 0;

        public required string Status { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}