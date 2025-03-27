namespace EMS.EMS.Domain.Entities
{
    public class TokenBlacklist
    {
        public int Id { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiryDate { get; set; }
    }
}