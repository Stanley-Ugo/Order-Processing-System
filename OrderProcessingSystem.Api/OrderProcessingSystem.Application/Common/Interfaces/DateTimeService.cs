namespace OrderProcessingSystem.Application.Common.Interfaces
{
    public class DateTimeService : IDateTime
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
