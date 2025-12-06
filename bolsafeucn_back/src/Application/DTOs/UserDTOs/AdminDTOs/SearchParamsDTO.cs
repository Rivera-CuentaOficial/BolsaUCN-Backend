namespace bolsafeucn_back.src.Application.DTOs.UserDTOs.AdminDTOs
{
    public class SearchParamsDTO
    {
        public string? SearchTerm { get; set; }
        public string? SortBy { get; set; }
        public string? SortOrder { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}