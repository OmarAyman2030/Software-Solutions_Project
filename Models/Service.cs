namespace Software_Solutions.Models
{
    public class Service
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string EstimatedTime { get; set; }
        public string Category { get; set; }

        public string? ImageUrl { get; set; }

    }
}
