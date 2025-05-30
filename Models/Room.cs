namespace backend.Models
{
    public class Room
    {
        public int Id { get; set; }
        public required string RoomName { get; set; }
        public string? Description { get; set; }
        public int IsAvailable { get; set; }
    }
}