using bolsafeucn_back.src.Application.DTOs.UserDTOs.AdminDTOs;
using bolsafeucn_back.src.Application.Services.Interfaces;
using bolsafeucn_back.src.Domain.Models;
using bolsafeucn_back.src.Infrastructure.Repositories.Interfaces;
using Serilog;

namespace bolsafeucn_back.src.Application.Services.Implements
{
    public class AdminService : IAdminService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;

        public AdminService(IUserRepository userRepository, ITokenService tokenService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
        }

        /// <summary>
        /// Alterna el estado de bloqueo de un usuario dado su ID, siempre y cuando el solicitante sea un administrador.
        /// </summary>
        /// <param name="adminId">ID del administrador que realiza la acción.</param>
        /// <param name="userId">ID del usuario cuyo estado de bloqueo se desea alternar.</param>
        /// <returns>El nuevo estado de bloqueo del usuario.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="KeyNotFoundException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="Exception"></exception>
        public async Task<bool> ToggleUserBlockedStatusAsync(int adminId, int userId)
        {
            //Chequeo de autobloqueo
            if (userId == adminId) 
            {
                Log.Warning("Un administrador intentó alternar su propio estado de bloqueo.");
                throw new InvalidOperationException("Un administrador no puede bloquear o desbloquearse a sí mismo.");
            }
            Log.Information($"Administrador con ID {adminId} está intentando alternar el estado de bloqueo del usuario con ID {userId}.");

            // Verificar que el solicitante es un administrador
            var admin = await _userRepository.GetByIdAsync(adminId);
            if (admin == null)
            {
                Log.Warning($"No se encontró al administrador con ID {adminId}.");
                throw new KeyNotFoundException("Administrador no encontrado.");
            }
            if (admin.UserType != UserType.Administrador)
            {
                Log.Warning($"El usuario con ID {adminId} no tiene permisos de administrador.");
                throw new UnauthorizedAccessException("El usuario no tiene permisos de administrador.");
            }

            Log.Information($"Buscando al usuario con ID {userId} para alternar su estado de bloqueo.");

            // Obtener el usuario objetivo
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) // Verificar que el usuario existe
            {
                Log.Warning($"No se encontró al usuario con ID {userId}.");
                throw new KeyNotFoundException("Usuario no encontrado.");
            }
            if (user.UserType == UserType.Administrador && admin.Admin!.SuperAdmin) // Prevenir bloqueo de administradores si es que es el ultimo
            {
                var numberOfAdmins = _userRepository.GetNumberOfAdmins();
                if (numberOfAdmins.Result <= 1)
                {
                    Log.Warning("Intento de bloquear al último administrador.");
                    throw new InvalidOperationException("No se puede bloquear al último administrador.");
                }
                Log.Warning($"Intento de alternar el estado de bloqueo del usuario con ID {userId}, que es un administrador.");
                throw new InvalidOperationException("No se puede bloquear o desbloquear a un administrador.");
            }

            user.Banned = !user.Banned;

            var toggleResult = await _userRepository.UpdateAsync(user);
            if (toggleResult)
            {
                Log.Information($"El estado de bloqueo del usuario con ID {userId} ha sido alternado a {user.Banned}.");
                if (user.Banned)
                {
                    var revokeResult = await _tokenService.RevokeAllActiveTokensAsync(userId);
                    Log.Information(revokeResult
                        ? $"Tokens activos revocados para el usuario con ID {userId} tras ser bloqueado."
                        : $"No se pudieron revocar los tokens activos para el usuario con ID {userId} tras ser bloqueado.");
                }
                return user.Banned;
            }
            else
            {
                Log.Error($"Error al actualizar el estado de bloqueo del usuario con ID {userId}.");
                throw new Exception("Error al actualizar el estado de bloqueo del usuario.");
            }
        }

        public async Task<UsersForAdminDTO> GetAllUsersAsync(int adminId)
        {
            var admin = await _userRepository.GetByIdAsync(adminId);
            if (admin == null)
            {
                Log.Warning($"No se encontró al administrador con ID {adminId}.");
                throw new KeyNotFoundException("Administrador no encontrado.");
            }
            if (admin.UserType != UserType.Administrador)
            {
                Log.Warning($"El usuario con ID {adminId} no tiene permisos de administrador.");
                throw new UnauthorizedAccessException("El usuario no tiene permisos de administrador.");
            }
            throw new NotImplementedException();
        }
    }
}