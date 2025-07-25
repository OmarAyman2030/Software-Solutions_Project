namespace Software_Solutions.DTO
{
    public class ServiceDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string EstimatedTime { get; set; }
        public string Category { get; set; }

        public string? ImageUrl { get; set; }
    }
}
