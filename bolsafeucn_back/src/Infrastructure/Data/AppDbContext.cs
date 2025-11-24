using bolsafeucn_back.src.Application.DTOs.ReviewDTO;
using bolsafeucn_back.src.Application.Services.Implements;
using bolsafeucn_back.src.Domain.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace bolsafeucn_back.src.Infrastructure.Data
{
    /// <summary>
    /// Contexto de base de datos principal de la aplicación
    /// Hereda de IdentityDbContext para incluir las funcionalidades de autenticación
    /// </summary>
    public class AppDbContext : IdentityDbContext<GeneralUser, Role, int>
    {
        private readonly ILogger<AppDbContext>? _logger;

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public AppDbContext(
            DbContextOptions<AppDbContext> options,
            ILogger<AppDbContext> logger
        )
            : base(options)
        {
            _logger = logger;
        }

        // DbSets - Representan las tablas en la base de datos
        public DbSet<Image> Images { get; set; }
        public DbSet<UserImage> UserImages { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Individual> Individuals { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<VerificationCode> VerificationCodes { get; set; }
        public DbSet<Publication> Publications { get; set; }
        public DbSet<Offer> Offers { get; set; }
        public DbSet<BuySell> BuySells { get; set; }
        public DbSet<NotificationDTO> Notifications { get; set; }
        public DbSet<JobApplication> JobApplications { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<AdminNotification> AdminNotifications { get; set; }


        // public DbSet<Review> Reviews { get; set; } // Desactivado temporalmente

        /// <summary>
        /// Configura las relaciones entre entidades y otras configuraciones de EF Core
        /// </summary>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Relaciones uno a uno entre GeneralUser y tipos específicos de usuario

            // Relación Student - Un usuario puede ser un estudiante
            builder
                .Entity<Student>()
                .HasOne(s => s.GeneralUser)
                .WithOne(gu => gu.Student)
                .HasForeignKey<Student>(s => s.GeneralUserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relación Company - Un usuario puede ser una empresa
            builder
                .Entity<Company>()
                .HasOne(c => c.GeneralUser)
                .WithOne(gu => gu.Company)
                .HasForeignKey<Company>(c => c.GeneralUserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relación Admin - Un usuario puede ser un administrador
            builder
                .Entity<Admin>()
                .HasOne(a => a.GeneralUser)
                .WithOne(gu => gu.Admin)
                .HasForeignKey<Admin>(a => a.GeneralUserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relación Individual - Un usuario puede ser un particular
            builder
                .Entity<Individual>()
                .HasOne(i => i.GeneralUser)
                .WithOne(gu => gu.Individual)
                .HasForeignKey<Individual>(i => i.GeneralUserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relaciones de JobApplication (Postulación a oferta)
            // Un estudiante puede hacer muchas postulaciones
            builder
                .Entity<JobApplication>()
                .HasOne(ja => ja.Student)
                .WithMany()
                .HasForeignKey(ja => ja.StudentId);

            // Una oferta puede tener muchas postulaciones
            builder
                .Entity<JobApplication>()
                .HasOne(ja => ja.JobOffer)
                .WithMany()
                .HasForeignKey(ja => ja.JobOfferId);

            // Relaciones de Publication (clase base para ofertas y compra/venta)
            // Un usuario puede crear muchas publicaciones
            builder
                .Entity<Publication>()
                .HasOne(p => p.User)
                .WithMany(u => u.Publications)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relaciones de Review
            // TODO: Implementar eliminacion de filas despues de probar que funciona todo.
            // Review - Publication (1:1)
            // Una publicación puede tener una review
            builder
                .Entity<Review>()
                .HasOne(r => r.Publication)
                .WithOne()
                .HasForeignKey<Review>(r => r.PublicationId);
            // .OnDelete(DeleteBehavior.Cascade);

            // Review - Student (Many:1)
            // Un estudiante puede tener muchas reviews (como estudiante evaluado)
            builder
                .Entity<Review>()
                .HasOne(r => r.Student)
                .WithMany()
                .HasForeignKey(r => r.StudentId);
            // .OnDelete(DeleteBehavior.Restrict);

            // Review - Offeror (Many:1)
            // Un proveedor puede tener muchas reviews (como oferente evaluado)
            builder
                .Entity<Review>()
                .HasOne(r => r.Offeror)
                .WithMany()
                .HasForeignKey(r => r.OfferorId)
                .OnDelete(DeleteBehavior.Restrict); // TODO: Revisar luego logica de Delete

            // Relaciones de UserImage (Imágenes de usuario)
            // Relación uno a uno entre GeneralUser y sus imágenes de perfil y banner
            builder
                .Entity<GeneralUser>()
                .HasOne(gu => gu.ProfilePhoto)
                .WithOne()
                .HasForeignKey<GeneralUser>(gu => gu.ProfilePhotoId)
                .OnDelete(DeleteBehavior.SetNull);
            builder
                .Entity<GeneralUser>()
                .HasOne(gu => gu.ProfileBanner)
                .WithOne()
                .HasForeignKey<GeneralUser>(gu => gu.ProfileBannerId)
                .OnDelete(DeleteBehavior.SetNull);

            // --- Fix: almacenar ApplicationStatus como string en la base de datos ---
            builder
                .Entity<JobApplication>()
                .Property(j => j.Status)
                .HasConversion<string>();
        }

        /// <summary>
        /// Override de SaveChangesAsync para detectar cambios en IsActive de publicaciones
        /// y crear automáticamente reviews iniciales cuando una publicación se cierra.
        /// </summary>
        public override async Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default
        )
        {
            // Detectar publicaciones que están siendo cerradas (IsActive: true -> false)
            var closedPublications = ChangeTracker
                .Entries<Publication>()
                .Where(e =>
                    e.State == EntityState.Modified
                    && e.Property(p => p.IsActive).IsModified
                    && e.Property(p => p.IsActive).CurrentValue == false
                    && e.Property(p => p.IsActive).OriginalValue == true
                )
                .Select(e => e.Entity)
                .ToList();
            // Guardar cambios
            var result = await base.SaveChangesAsync(cancellationToken);
            if (closedPublications.Count != 0) // Si hay publicaciones cerradas
            {
                await CreateReviewsForClosedPublicationsAsync(
                    closedPublications,
                    cancellationToken
                );
            }
            return result;
        }

        /// <summary>
        /// Crea reviews iniciales para todas las postulaciones aceptadas
        /// de las publicaciones que acaban de cerrarse.
        /// </summary>
        private async Task CreateReviewsForClosedPublicationsAsync(
            List<Publication> closedPublications,
            CancellationToken cancellationToken
        )
        {
            foreach (var publication in closedPublications)
            {
                try
                {
                    _logger?.LogInformation(
                        "Procesando cierre de publicación ID: {PublicationId} - {Title}",
                        publication.Id,
                        publication.Title
                    );
                    // TODO: Preguntar logica para BuySell
                    // Por ahora solo se procesa Offers que tienen postulaciones
                    if (publication is not Offer offer)
                    {
                        _logger?.LogInformation(
                            "Publicación ID: {PublicationId} es tipo {Type}, no tiene postulaciones para crear reviews",
                            publication.Id,
                            publication.GetType().Name
                        );
                        continue;
                    }
                    // Obtener postulaciones aceptadas para esta Offer
                    var acceptedPostulations = await JobApplications
                        .Where(ja =>
                            ja.JobOfferId == offer.Id && ja.Status == ApplicationStatus.Aceptada
                        )
                        .ToListAsync(cancellationToken);
                    if (acceptedPostulations.Count == 0)
                    {
                        _logger?.LogInformation(
                            "No hay postulaciones aceptadas para la oferta ID: {OfferId}",
                            offer.Id
                        );
                        continue;
                    }
                    _logger?.LogInformation(
                        "Encontradas {Count} postulaciones aceptadas para crear reviews",
                        acceptedPostulations.Count
                    );
                    // Crear review por cada postulación aceptada
                    foreach (var postulation in acceptedPostulations)
                    {
                        // Evitar duplicados
                        var existingReview = await Reviews.AnyAsync(
                            r =>
                                r.PublicationId == offer.Id
                                && r.StudentId == postulation.StudentId
                                && r.OfferorId == offer.UserId,
                            cancellationToken
                        );
                        if (existingReview)
                        {
                            _logger?.LogWarning(
                                "Ya existe una review para Oferta: {OfferId}, Estudiante: {StudentId}, Oferente: {OfferorId}",
                                offer.Id,
                                postulation.StudentId,
                                offer.UserId
                            );
                            continue;
                        }
                        var review = new Review
                        {
                            PublicationId = offer.Id,
                            StudentId = postulation.StudentId,
                            OfferorId = offer.UserId,
                            ReviewWindowEndDate = DateTime.UtcNow.AddDays(14),
                            IsReviewForStudentCompleted = false,
                            IsReviewForOfferorCompleted = false,
                            IsCompleted = false,
                            IsClosed = false,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow,
                        };
                        Reviews.Add(review);
                        _logger?.LogInformation(
                            "Review creada para Estudiante ID: {StudentId}, y Oferente ID: {OfferorId} en Oferta ID: {OfferId}",
                            postulation.StudentId,
                            offer.UserId,
                            offer.Id
                        );
                    }
                    // Guardar las reviews creadas
                    await base.SaveChangesAsync(cancellationToken);
                    _logger?.LogInformation(
                        "Se crearon {Count} reviews iniciales para la oferta ID: {OfferId}",
                        acceptedPostulations.Count,
                        offer.Id
                    );
                }
                catch (Exception ex)
                {
                    _logger?.LogError(
                        ex,
                        "Error al crear reviews para publicación ID: {PublicationId}",
                        publication.Id
                    );
                    // No lanzar la excepción para no romper el flujo principal
                    // Las reviews se pueden crear manualmente si falla
                }
            }
        }
    }
}
