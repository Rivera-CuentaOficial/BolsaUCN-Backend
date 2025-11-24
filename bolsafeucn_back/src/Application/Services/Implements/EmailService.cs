using bolsafeucn_back.src.Application.Services.Interfaces;
using bolsafeucn_back.src.Application.DTOs.ReviewDTO;
using Resend;
using Serilog;

namespace bolsafeucn_back.src.Application.Services.Implements
{
    public class EmailService : IEmailService
    {
        private readonly IResend _resend;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;
        

        public EmailService(
            IResend resend,
            IConfiguration configuration,
            IWebHostEnvironment environment
        )
        {
            _resend = resend;
            _configuration = configuration;
            _environment = environment;
        }

        // --------------------------------------------------------------------
        // 1. EMAIL DE VERIFICACIÓN
        // --------------------------------------------------------------------
        public async Task<bool> SendVerificationEmailAsync(string email, string code)
        {
            try
            {
                Log.Information("Iniciando envío de email de verificación a: {Email}", email);
                var htmlBody = await LoadTemplateAsync("VerificationEmail", code);

                var message = new EmailMessage
                {
                    To = email,
                    From = _configuration["EmailConfiguration:From"]!,
                    Subject = _configuration["EmailConfiguration:VerificationSubject"]!,
                    HtmlBody = htmlBody,
                };

                var result = await _resend.EmailSendAsync(message);

                if (!result.Success)
                {
                    Log.Error("El envío del email de verificación falló para: {Email}", email);
                    return false;
                }

                Log.Information("Email de verificación enviado exitosamente a: {Email}", email);
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al enviar email de verificación a: {Email}", email);
                return false;
            }
        }

        // --------------------------------------------------------------------
        // 2. EMAIL RESETEO DE CONTRASEÑA
        // --------------------------------------------------------------------
        public async Task<bool> SendResetPasswordVerificationEmailAsync(string email, string code)
        {
            try
            {
                Log.Information("Iniciando envío de email de restablecimiento a: {Email}", email);
                var htmlBody = await LoadTemplateAsync("PasswordResetEmail", code);

                var message = new EmailMessage
                {
                    To = email,
                    From = _configuration["EmailConfiguration:From"]!,
                    Subject = _configuration["EmailConfiguration:PasswordResetSubject"]!,
                    HtmlBody = htmlBody,
                };

                await _resend.EmailSendAsync(message);

                Log.Information("Email de restablecimiento enviado exitosamente a: {Email}", email);
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al enviar email de restablecimiento a: {Email}", email);
                return false;
            }
        }

        // --------------------------------------------------------------------
        // 3. EMAIL DE BIENVENIDA
        // --------------------------------------------------------------------
        public async Task<bool> SendWelcomeEmailAsync(string email)
        {
            try
            {
                Log.Information("Iniciando envío de email de bienvenida a: {Email}", email);
                var htmlBody = await LoadTemplateAsync("WelcomeEmail", null);

                var message = new EmailMessage
                {
                    From = _configuration["EmailConfiguration:From"]!,
                    To = email,
                    Subject = _configuration["EmailConfiguration:WelcomeSubject"]!,
                    HtmlBody = htmlBody,
                };

                await _resend.EmailSendAsync(message);

                Log.Information("Email de bienvenida enviado exitosamente a: {Email}", email);
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al enviar email de bienvenida a: {Email}", email);
                return false;
            }
        }

        // --------------------------------------------------------------------
        // 4. TEMPLATE LOADER GENERAL
        // --------------------------------------------------------------------
        public async Task<string> LoadTemplateAsync(string templateName, string? code)
        {
            try
            {
                var templatePath = Path.Combine(
                    _environment.ContentRootPath,
                    "src",
                    "Application",
                    "Templates",
                    "Emails",
                    $"{templateName}.html"
                );

                Log.Debug("Cargando template de email: {TemplateName} desde {Path}", templateName, templatePath);

                var htmlContent = await File.ReadAllTextAsync(templatePath);

                if (code != null)
                    htmlContent = htmlContent.Replace("{{CODE}}", code);

                return htmlContent;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al cargar template de email: {TemplateName}", templateName);
                throw;
            }
        }

        // --------------------------------------------------------------------
        // 5. EMAIL CAMBIO DE ESTADO DE POSTULACIÓN
        // --------------------------------------------------------------------
        public async Task<bool> SendPostulationStatusChangeEmailAsync(string email, string offerName, string companyName, string newStatus)
        {
            try
            {
                Log.Information("Enviando email de cambio de estado a {Email}", email);

                var htmlBody = await LoadPostulationStatusTemplateAsync(offerName, companyName, newStatus);

                var message = new EmailMessage
                {
                    To = email,
                    From = _configuration["EmailConfiguration:From"]!,
                    Subject = "Actualización en tu postulación",
                    HtmlBody = htmlBody
                };

                var result = await _resend.EmailSendAsync(message);

                if (!result.Success)
                {
                    Log.Error("Error al enviar correo de cambio de estado a {Email}", email);
                    return false;
                }

                Log.Information("Correo de cambio de estado enviado exitosamente a {Email}", email);
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error enviando correo de cambio de estado a {Email}", email);
                return false;
            }
        }

        private async Task<string> LoadPostulationStatusTemplateAsync(string offerName, string companyName, string newStatus)
        {
            try
            {
                var templatePath = Path.Combine(
                    _environment.ContentRootPath,
                    "src",
                    "Application",
                    "Templates",
                    "Emails",
                    "PostulationStatusChanged.html"
                );

                var html = await File.ReadAllTextAsync(templatePath);

                html = html.Replace("{{OFFER_NAME}}", offerName);
                html = html.Replace("{{COMPANY_NAME}}", companyName);
                html = html.Replace("{{NEW_STATUS}}", newStatus);

                return html;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error cargando template PostulationStatusChanged");
                throw;
            }
        }

        // --------------------------------------------------------------------
        // 6. *** NUEVO *** EMAIL PARA REVIEW ≤ 3 (EVA-006)
        // --------------------------------------------------------------------
        public async Task<bool> SendLowRatingReviewAlertAsync(ReviewDTO review)
        {
            try
            {
                Log.Information("Enviando alerta de review baja al admin");

                string adminEmail = _configuration["AdminNotifications:Email"]!;
                string fromEmail = _configuration["EmailConfiguration:From"]!;

                // Determinar qué tipo de reseña es y su comentario
                int? rating = review.RatingForStudent ?? review.RatingForOfferor;
                string? comment = review.CommentForStudent ?? review.CommentForOfferor;

                string htmlBody = $@"
                    <h2>Alerta: Nueva reseña crítica</h2>

                    <p><strong>Puntaje:</strong> {rating}</p>
                    <p><strong>Comentario:</strong> {comment}</p>

                    <p><strong>Id Estudiante:</strong> {review.IdStudent}</p>
                    <p><strong>Id Oferente:</strong> {review.IdOfferor}</p>
                    <p><strong>Id Publicación:</strong> {review.IdPublication}</p>

                    <p><strong>¿Llegó a tiempo?:</strong> {(review.AtTime ? "Sí" : "No")}</p>
                    <p><strong>Buena presentación?:</strong> {(review.GoodPresentation ? "Sí" : "No")}</p>

                    <p><strong>Ventana de revisión cierra:</strong> {review.ReviewWindowEndDate}</p>
                ";

                var message = new EmailMessage
                {
                    To = adminEmail,
                    From = fromEmail,
                    Subject = "[ALERTA] Nueva reseña crítica (≤ 3 estrellas)",
                    HtmlBody = htmlBody
                };

                var result = await _resend.EmailSendAsync(message);

                if (!result.Success)
                {
                    Log.Error("Error enviando alerta de review baja.");
                    return false;
                }

                Log.Information("Alerta de review baja enviada exitosamente al admin.");
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error enviando alerta de review baja");
                return false;
            }
        }

        }
    }
