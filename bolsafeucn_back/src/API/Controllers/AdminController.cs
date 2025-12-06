using bolsafeucn_back.src.Application.DTOs.BaseResponse;
using bolsafeucn_back.src.Application.DTOs.UserDTOs.AdminDTOs;
using bolsafeucn_back.src.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace bolsafeucn_back.src.API.Controllers
{
    public class AdminController : BaseController
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        /// <summary>
        /// Alterna el estado de bloqueo de un usuario.
        /// </summary>
        /// <returns>Respuesta genérica indicando el resultado de la operación.</returns>
        [HttpPatch("users/toggle-block/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ToggleUserBlockedStatus([FromRoute] int userId)
        {
            var adminId = GetUserIdFromToken();
            Log.Information($"Intentando alternar el estado de bloqueo del usuario con ID {userId}");
            var result = await _adminService.ToggleUserBlockedStatusAsync(adminId, userId);
            return Ok(new GenericResponse<bool>("Estado de bloqueo del usuario actualizado.", result));
        }

        [HttpGet("users")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers([FromQuery] SearchParamsDTO searchParams)
        {
            var adminId = GetUserIdFromToken();
            Log.Information("Obteniendo todos los usuarios.");
            var users = await _adminService.GetAllUsersAsync(adminId, searchParams);
            return Ok(new GenericResponse<UsersForAdminDTO>("Usuarios obtenidos exitosamente.", users));
        }
    }
}