using bolsafeucn_back.src.Application.DTOs.BaseResponse;
using bolsafeucn_back.src.Application.DTOs.PublicationDTO;
using bolsafeucn_back.src.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace bolsafeucn_back.src.API.Controllers
{
    [ApiController]
    [Route("api/publications")]
    public class BuySellController : BaseController
    {
        private readonly IPublicationService _publicationService;

        public BuySellController(IPublicationService publicationService)
        {
            _publicationService = publicationService;
        }

        public async Task<IActionResult> CreateBuySell([FromBody] CreateBuySellDTO buySellDto)
        {
            int parsedUserId = GetUserIdFromToken();
            var result = await _publicationService.CreateBuySellAsync(buySellDto, parsedUserId);
            return Ok(
                new GenericResponse<string>(
                    "Publicación de compra/venta creada exitosamente.",
                    result
                )
            );
        }
    }
}
