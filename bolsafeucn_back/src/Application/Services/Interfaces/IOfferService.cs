using bolsafeucn_back.src.Application.DTOs.OfferDTOs;
using bolsafeucn_back.src.Application.DTOs.PublicationDTO;

namespace bolsafeucn_back.src.Application.Services.Interfaces;

public interface IOfferService
{
    Task<OfferDetailDto?> GetOfferDetailsAsync(int offerId);
    Task<IEnumerable<OfferSummaryDto>> GetActiveOffersAsync();
    Task PublishOfferAsync(int id);
    Task RejectOfferAsync(int id);
    Task<IEnumerable<PendingOffersForAdminDto>> GetPendingOffersAsync();
    Task<IEnumerable<OfferBasicAdminDto>> GetPublishedOffersAsync();
    Task<OfferDetailsAdminDto> GetOfferDetailsForAdminManagement(int offerId);
    Task GetOfferForAdminToClose(int offerId);
    Task<OfferDetailValidationDto> GetOfferDetailForOfferValidationAsync(int id);
    Task GetOfferForAdminToPublish(int id);
    Task GetOfferForAdminToReject(int id);
    Task<OfferDetailDto> GetOfferDetailForOfferer(int id);

}
