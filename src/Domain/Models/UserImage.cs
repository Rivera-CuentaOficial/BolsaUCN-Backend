namespace bolsafeucn_back.src.Domain.Models
{
    public class UserImage : ModelBase
    {
        public required string Url { get; set; }
        public required string PublicId { get; set; }
    }
}
