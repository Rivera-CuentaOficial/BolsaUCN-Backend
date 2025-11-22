using bolsafeucn_back.src.Application.DTOs.PublicationDTO;
using bolsafeucn_back.src.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace bolsafeucn_back.src.API.Controllers
{
    [Authorize(Roles = "Admin")] // Solo el rol 'Admin' puede usar estos endpoints
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : BaseController
    {
        private readonly IPublicationService _publicationService;

        public AdminController(IPublicationService publicationService)
        {
            _publicationService = publicationService;
        }

        /// <summary>
        /// Approves a publication (Offer or BuySell), making it visible to all users.
        /// </summary>
        /// <param name="id">The ID of the publication to approve.</param>
        [HttpPut("publications/{id}/approve")]
        public async Task<IActionResult> ApprovePublication(int id)
        {
            // El middleware manejará las excepciones (404, etc.)
            var response = await _publicationService.AdminApprovePublicationAsync(id);
            return Ok(response);
        }

        /// <summary>
        /// Rejects a publication. Requires a reason/comment for the rejection.
        /// </summary>
        /// <param name="id">The ID of the publication to reject.</param>
        /// <param name="dto">The rejection reason.</param>
        [HttpPut("publications/{id}/reject")]
        public async Task<IActionResult> RejectPublication(int id, [FromBody] AdminRejectDto dto)
        {
            // El servicio valida que el DTO tenga motivo.
            var response = await _publicationService.AdminRejectPublicationAsync(id, dto);
            return Ok(response);
        }
    }
}