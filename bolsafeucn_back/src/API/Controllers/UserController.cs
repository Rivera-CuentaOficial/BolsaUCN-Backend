using System.Security.Claims;
using bolsafeucn_back.src.Application.DTOs.BaseResponse;
using bolsafeucn_back.src.Application.DTOs.UserDTOs.UserProfileDTOs;
using bolsafeucn_back.src.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace bolsafeucn_back.src.API.Controllers
{
    public class UserController : BaseController
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("profile")] //No estoy seguro si este es el endpoint correcto
        [Authorize]
        public async Task<IActionResult> GetUserProfile()
        {
            Log.Information("Verificando token de autenticacion");
            var userId =
                (
                    User.Identity?.IsAuthenticated == true
                        ? User
                            .Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)
                            ?.Value
                        : null
                ) ?? throw new UnauthorizedAccessException("Usuario no autenticado.");

            int.TryParse(userId, out int parsedUserId);
            var result = await _userService.GetUserProfileByIdAsync(parsedUserId);
            return Ok(new GenericResponse<GetUserProfileDTO>("Datos de perfil obtenidos.", result));
        }
    }
}