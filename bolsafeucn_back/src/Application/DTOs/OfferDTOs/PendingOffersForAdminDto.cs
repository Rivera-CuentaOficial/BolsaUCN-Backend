using bolsafeucn_back.src.Domain.Models;

namespace bolsafeucn_back.src.Application.DTOs.OfferDTOs
{
    /// <summary>
    /// DTO containing minimal information about offers pending admin validation.
    /// Used to populate admin review lists.
    /// </summary>
    public class PendingOffersForAdminDto
    {
        /// <summary>
        /// Offer title.
        /// </summary>
        public required string Title { get; set; }

        /// <summary>
        /// Publication type (Offer, BuySell).
        /// </summary>
        public Types Type { get; set; }
    }
}