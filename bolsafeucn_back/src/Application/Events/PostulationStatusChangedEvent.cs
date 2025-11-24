using System.ComponentModel.DataAnnotations;
using bolsafeucn_back.src.Domain.Models;

public class PostulationStatusChangedEvent
{
    public int PostulationId { get; set; }

    [Required(ErrorMessage = "El estado es requerido")]
    [EnumDataType(typeof(ApplicationStatus), ErrorMessage = "El estado debe ser: Pendiente, Aceptada o Rechazada")]
    public ApplicationStatus NewStatus { get; set; }

    [Required(ErrorMessage = "El nombre de la oferta es requerido")]
    public required string OfferName { get; set; }

    [Required(ErrorMessage = "El nombre de la compañía es requerido")]
    public required string CompanyName { get; set; }

    [Required(ErrorMessage = "El email del estudiante es requerido")]
    [EmailAddress(ErrorMessage = "El email no es válido")]
    public required string StudentEmail { get; set; }
}
