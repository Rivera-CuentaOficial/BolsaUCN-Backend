using bolsafeucn_back.src.Application.Services.Interfaces;
using bolsafeucn_back.src.Domain.Models;

namespace bolsafeucn_back.src.Application.Services.Implements
{
    /// <summary>
    /// Servicio de almacenamiento local de documentos.
    /// </summary>
    public class CloudStorageService : IDocumentStorageProvider
    {
        public async Task<bool> UploadCVAsync(IFormFile cvFile, User user)
        {
            // Implementación para subir el CV al almacenamiento local
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteCVAsync(User user)
        {
            // Implementación para eliminar el CV del almacenamiento local
            throw new NotImplementedException();
        }

        public async Task<Curriculum?> DownloadCVAsync(User user)
        {
            // Implementación para descargar el CV del almacenamiento local
            throw new NotImplementedException();
        }

        public async Task<bool> CVExistsAsync(User user)
        {
            // Implementación para verificar si el CV existe en el almacenamiento local
            throw new NotImplementedException();
        }
    }
}
