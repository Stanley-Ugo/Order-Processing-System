namespace OrderProcessingSystem.Domain.Common
{
    public class OutboxMessage
    {
        public Guid Id { get; set; }
        public string Type { get; set; } = default!;
        public string Content { get; set; } = default!;
        public DateTime OccurredOn { get; set; }
        public DateTime? ProcessedOn { get; set; }
        public string? Error { get; set; }
    }
}
