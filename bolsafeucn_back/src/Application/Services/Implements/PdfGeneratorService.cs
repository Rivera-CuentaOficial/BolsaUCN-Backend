using bolsafeucn_back.src.Application.DTOs.ReviewDTO.ReviewReport;
using bolsafeucn_back.src.Application.Services.Interfaces;
using bolsafeucn_back.src.Domain.Models;
using bolsafeucn_back.src.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace bolsafeucn_back.src.Application.Services.Implements
{
    /// <summary>
    /// Servicio para la generación de documentos PDF con información de reviews
    /// </summary>
    public class PdfGeneratorService : IPdfGeneratorService
    {
        private readonly AppDbContext _context;
        private readonly IReviewService _reviewService;

        public PdfGeneratorService(AppDbContext context, IReviewService reviewService)
        {
            _context = context;
            _reviewService = reviewService;

            // Configuración de licencia QuestPDF (Community - gratuita para uso educativo)
            QuestPDF.Settings.License = LicenseType.Community;
        }

        /// <summary>
        /// Genera un PDF con todas las reviews del usuario
        /// </summary>
        public async Task<byte[]> GenerateUserReviewsPdfAsync(int userId)
        {
            // 1. Obtener usuario
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId) ?? throw new KeyNotFoundException($"Usuario {userId} no encontrado");

            // 2. Determinar si es estudiante u oferente
            var isStudent = user.UserType == UserType.Estudiante;

            // 3. Obtener reviews
            var reviewsDto = isStudent
                ? await _reviewService.GetReviewsByStudentAsync(userId)
                : await _reviewService.GetReviewsByOfferorAsync(userId);

            // 4. Obtener datos detallados de reviews desde la BD
            var reviewIds = reviewsDto.Select(r => r.idReview).ToList();
            var reviews = await _context.Reviews
                .Include(r => r.Publication)
                .Include(r => r.Student)
                .Include(r => r.Offeror)
                .Where(r => reviewIds.Contains(r.Id))
                .ToListAsync();

            // 5. Construir DTO para el reporte
            var reportData = new ReviewReportDTO
            {
                UserName = user.UserName ?? "Usuario",
                UserEmail = user.Email ?? "N/A",
                AverageRating = user.Rating, // Obtenido directamente del usuario
                TotalReviews = reviewsDto.Count(),
                GeneratedAt = DateTime.UtcNow,
                Reviews = reviews.Select(r => new ReviewDetailDTO
                {
                    ReviewId = r.Id,
                    PublicationTitle = r.Publication?.Title ?? "Publicación no disponible",
                    Rating = isStudent ? r.RatingForStudent : r.RatingForOfferor,
                    Comment = isStudent ? r.CommentForStudent : r.CommentForOfferor,
                    ReviewDate = r.CreatedAt,
                    ReviewerName = isStudent
                        ? (r.Offeror?.UserName ?? "Oferente")
                        : (r.Student?.UserName ?? "Estudiante"),
                    AtTime = isStudent ? r.AtTime : null,
                    GoodPresentation = isStudent ? r.GoodPresentation : null
                }).ToList()
            };

            // 6. Generar PDF
            return GeneratePdfDocument(reportData, isStudent);
        }

        /// <summary>
        /// Genera el documento PDF con QuestPDF
        /// </summary>
        private byte[] GeneratePdfDocument(ReviewReportDTO data, bool isStudent)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    // Header
                    page.Header().Element(c => ComposeHeader(c, data));

                    // Content
                    page.Content().Element(c => ComposeContent(c, data, isStudent));

                    // Footer
                    page.Footer().AlignCenter().Text($"Generado el: {data.GeneratedAt:dd/MM/yyyy HH:mm} | BolsaFEUCN")
                        .FontSize(9)
                        .FontColor(Colors.Grey.Medium);
                });
            });

            return document.GeneratePdf();
        }

        /// <summary>
        /// Compone el header del PDF
        /// </summary>
        private void ComposeHeader(IContainer container, ReviewReportDTO data)
        {
            container.Column(column =>
            {
                column.Item().Text("Reporte de Calificaciones")
                    .FontSize(20)
                    .Bold()
                    .FontColor(Colors.Blue.Darken2);

                column.Item().PaddingTop(5).Text(data.UserName)
                    .FontSize(14)
                    .SemiBold();

                column.Item().Text(data.UserEmail)
                    .FontSize(10)
                    .FontColor(Colors.Grey.Darken1);

                column.Item().PaddingTop(10).BorderBottom(1).BorderColor(Colors.Grey.Lighten1);
            });
        }

        /// <summary>
        /// Compone el contenido principal del PDF
        /// </summary>
        private void ComposeContent(IContainer container, ReviewReportDTO data, bool isStudent)
        {
            container.PaddingVertical(20).Column(column =>
            {
                // Sección de resumen
                column.Item().Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text("Promedio General").FontSize(12).SemiBold();
                        col.Item().Text($"{data.AverageRating:F2}/6.0")
                            .FontSize(24)
                            .Bold()
                            .FontColor(GetRatingColor(data.AverageRating));
                    });

                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text("Total de Reviews").FontSize(12).SemiBold();
                        col.Item().Text(data.TotalReviews.ToString())
                            .FontSize(24)
                            .Bold()
                            .FontColor(Colors.Blue.Medium);
                    });
                });

                column.Item().PaddingTop(20).Text("Detalle de Calificaciones")
                    .FontSize(14)
                    .SemiBold();

                // Verificar si hay reviews
                if (!data.Reviews.Any())
                {
                    column.Item().PaddingTop(20).Text("No hay calificaciones registradas.")
                        .FontSize(12)
                        .Italic()
                        .FontColor(Colors.Grey.Medium);
                }
                else
                {
                    // Lista de reviews
                    foreach (var review in data.Reviews)
                    {
                        column.Item().PaddingTop(15).Element(c => ComposeReviewCard(c, review, isStudent));
                    }
                }
            });
        }

        /// <summary>
        /// Compone una tarjeta individual de review
        /// </summary>
        private void ComposeReviewCard(IContainer container, ReviewDetailDTO review, bool isStudent)
        {
            container.Border(1).BorderColor(Colors.Grey.Lighten2)
                .Padding(15)
                .Column(column =>
                {
                    // Título publicación
                    column.Item().Text(review.PublicationTitle)
                        .FontSize(12)
                        .SemiBold();

                    // Rating
                    if (review.Rating.HasValue)
                    {
                        column.Item().PaddingTop(5).Row(row =>
                        {
                            row.AutoItem().Text("Calificación: ");
                            row.AutoItem().Text($"{review.Rating.Value}/6")
                                .Bold()
                                .FontColor(GetRatingColor(review.Rating.Value));
                        });
                    }
                    else
                    {
                        column.Item().PaddingTop(5).Text("Calificación: Sin calificar")
                            .FontColor(Colors.Grey.Medium)
                            .Italic();
                    }

                    // Comentario
                    if (!string.IsNullOrEmpty(review.Comment))
                    {
                        column.Item().PaddingTop(5).Row(row =>
                        {
                            row.AutoItem().Text("Comentario: ").FontSize(10);
                            row.AutoItem().Text(review.Comment).FontSize(10).Italic();
                        });
                    }

                    // Campos específicos para estudiantes
                    if (isStudent)
                    {
                        column.Item().PaddingTop(5).Row(row =>
                        {
                            if (review.AtTime.HasValue)
                            {
                                row.AutoItem().Text("• Puntualidad: ").FontSize(9);
                                row.AutoItem().Text(review.AtTime.Value ? "Sí" : "No")
                                    .FontSize(9)
                                    .FontColor(review.AtTime.Value ? Colors.Green.Medium : Colors.Red.Medium);
                                row.AutoItem().PaddingLeft(15);
                            }

                            if (review.GoodPresentation.HasValue)
                            {
                                row.AutoItem().Text("• Presentación: ").FontSize(9);
                                row.AutoItem().Text(review.GoodPresentation.Value ? "Buena" : "Regular")
                                    .FontSize(9)
                                    .FontColor(review.GoodPresentation.Value ? Colors.Green.Medium : Colors.Orange.Medium);
                            }
                        });
                    }

                    // Revisor y fecha
                    column.Item().PaddingTop(5)
                        .Text($"Por: {review.ReviewerName} | {review.ReviewDate:dd/MM/yyyy}")
                        .FontSize(9)
                        .FontColor(Colors.Grey.Medium);
                });
        }

        /// <summary>
        /// Obtiene el color según el rating
        /// </summary>
        private string GetRatingColor(double rating)
        {
            if (rating >= 5.5) return Colors.Green.Darken2;
            if (rating >= 4.5) return Colors.Green.Medium;
            if (rating >= 4.0) return Colors.Blue.Medium;
            if (rating >= 3.0) return Colors.Orange.Medium;
            return Colors.Red.Medium;
        }
    }
}
