namespace Software_Solutions.Models
{
    public class User
    {
        public int ID { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? PasswordHash { get; set; }
        public bool EmailConfirmed { get; set; } = false;
        public string Role { get; set; } = "User";
        public string? EmailVerficationToken { get; set; }
        public DateTime TokenExpiration { get; set; }
    }
}
