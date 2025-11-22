namespace bolsafeucn_back.src.Application.Services.Interfaces
{
    /// <summary>
    /// Interfaz para el servicio de generación de PDFs
    /// </summary>
    public interface IPdfGeneratorService
    {
        /// <summary>
        /// Genera un PDF con todas las reviews del usuario especificado.
        /// El PDF incluye un resumen con el promedio de calificación y el total de reviews,
        /// así como el detalle de cada review individual.
        /// </summary>
        /// <param name="userId">ID del usuario para generar el reporte</param>
        /// <returns>Array de bytes del PDF generado</returns>
        /// <exception cref="KeyNotFoundException">Si no se encuentra el usuario</exception>
        Task<byte[]> GenerateUserReviewsPdfAsync(int userId);
    }
}
