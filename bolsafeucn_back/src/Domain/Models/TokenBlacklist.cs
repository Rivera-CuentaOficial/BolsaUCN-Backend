namespace bolsafeucn_back.src.Domain.Models
{
    public class Blacklist : ModelBase
    {   
        /*
        public required int WhitelistId { get; set; }
        public required Whitelist Token { get; set; }
        */
        public required int UserId { get; set; }
        public required string Email { get; set; }
        public required string Token { get; set; }
    }
}