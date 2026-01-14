using System.Security.Claims;
using bolsafeucn_back.src.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace bolsafeucn_back.src.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BaseController : ControllerBase
    {
        /// <summary>
        /// Obtiene el ID y tipo de usuario desde el token de autenticación.
        /// </summary>
        /// <returns>Tupla con el ID y tipo de usuario.</returns>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="ArgumentException"></exception>
        protected (int, UserType) GetIdAndTypeFromToken()
        {
            Log.Information("Verificando token de autenticacion");
            if (User.Identity?.IsAuthenticated != true)
                throw new UnauthorizedAccessException("Usuario no autenticado.");
            var userId =
                User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? null;
            var userType = User.Claims.FirstOrDefault(c => c.Type == "userType")?.Value ?? null;
            int.TryParse(userId, out int parsedUserId);
            if (!Enum.TryParse<UserType>(userType, ignoreCase: true, out var parsedUserType))
                throw new ArgumentException("Tipo de usuario no existe");
            return (parsedUserId, parsedUserType);
        }

        /// <summary>
        /// Obtiene el ID del usuario desde el token de autenticación.
        /// </summary>
        /// <returns>ID del usuario.</returns>
        /// <exception cref="UnauthorizedAccessException"></exception>
        protected int GetUserIdFromToken()
        {
            if (User.Identity?.IsAuthenticated != true)
                throw new UnauthorizedAccessException("Usuario no autenticado.");
            var userId =
                User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? null;
            int.TryParse(userId, out int parsedUserId);
            return parsedUserId;
        }

        /// <summary>
        /// Obtiene el email del usuario desde el token de autenticación.
        /// </summary>
        /// <returns>Email del usuario.</returns>
        /// <exception cref="UnauthorizedAccessException"></exception>
        protected string GetEmailFromToken()
        {
            if (User.Identity?.IsAuthenticated != true)
                throw new UnauthorizedAccessException("Usuario no autenticado.");
            var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value ?? null;
            return email!;
        }

        /// <summary>
        /// Obtiene el tipo de usuario desde el token de autenticación.
        /// </summary>
        /// <returns>Tipo de usuario.</returns>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="ArgumentException"></exception>
        protected UserType GetTypeFromToken()
        {
            if (User.Identity?.IsAuthenticated != true)
                throw new UnauthorizedAccessException("Usuario no autenticado.");
            var userType = User.Claims.FirstOrDefault(c => c.Type == "userType")?.Value ?? null;
            if (!Enum.TryParse<UserType>(userType, ignoreCase: true, out var parsedUserType))
                throw new ArgumentException("Tipo de usuario no existe");
            return parsedUserType;
        }
    }
}
