namespace Journal.Entities
{
    public class JournalPin
    {
        public int Id { get; set; }
        public string PinHash { get; set; } = string.Empty;
        public DateTime UpdatedAt { get; set; }
    }
}
