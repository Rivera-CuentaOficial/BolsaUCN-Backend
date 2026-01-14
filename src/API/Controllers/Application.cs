using bolsafeucn_back.src.Application.DTOs.BaseResponse;
using bolsafeucn_back.src.Application.DTOs.JobAplicationDTO;
using bolsafeucn_back.src.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace bolsafeucn_back.src.API.Controllers
{
    [ApiController]
    [Route("api/publications")]
    public class ApplicationController : BaseController
    {
        private readonly IJobApplicationService _service;

        public ApplicationController(IJobApplicationService service)
        {
            _service = service;
        }

        /// <summary>
        /// Permite a un estudiante postularse a una oferta laboral.
        /// </summary>
        /// <param name="offerId">ID de la oferta laboral a la que el estudiante desea postularse.</param>
        /// <returns>Respuesta que entrega un DTO con la aplicación creada.</returns>
        [HttpPost("offers/{id}/apply")]
        [Authorize(Roles = "Applicant")]
        public async Task<IActionResult> ApplyToOffer(int offerId)
        {
            int parsedUserId = GetUserIdFromToken();
            var result = await _service.CreateApplicationAsync(parsedUserId, offerId);
            return Ok(
                new GenericResponse<JobApplicationResponseDto>(
                    "Aplicación creada exitosamente.",
                    result
                )
            );
        }

        /// <summary>
        /// Obtiene todas las aplicaciones realizadas por el estudiante autenticado.
        /// </summary>
        /// <returns>Respuesta que entrega un DTO con las aplicaciones del estudiante.</returns>
        [HttpGet("offers/my-applications")]
        [Authorize(Roles = "Applicant")]
        public async Task<IActionResult> GetMyApplications()
        {
            int parsedUserId = GetUserIdFromToken();
            var applications = await _service.GetStudentApplicationsAsync(parsedUserId);
            return Ok(
                new GenericResponse<IEnumerable<JobApplicationResponseDto>>(
                    "Aplicaciones obtenidas exitosamente.",
                    applications
                )
            );
        }
    }
}
