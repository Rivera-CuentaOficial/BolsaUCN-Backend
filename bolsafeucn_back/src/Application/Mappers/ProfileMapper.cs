using Mapster;
using bolsafeucn_back.src.Domain.Models;
using bolsafeucn_back.src.Application.DTOs.UserDTOs.UserProfileDTOs;
using System.IO.Compression;

namespace bolsafeucn_back.src.Application.Mappers
{
    /// <summary>
    /// Clase encargada de mapear entidades relacionadas con el perfil de usuario.
    /// </summary>
    public class ProfileMapper
    {
        /// <summary>
        /// Configura todos los mapeos relacionados con el perfil de usuario.
        /// </summary>
        public void ConfigureAllMappings()
        {
            ConfigureViewProfileMapping();
        }
        /// <summary>
        /// Configura el mapeo de GeneralUser a GetUserProfileDTO para ver el perfil de usuario.
        /// </summary>
        public void ConfigureViewProfileMapping()
        {
            TypeAdapterConfig<GeneralUser, GetUserProfileDTO>
                .NewConfig()
                .Map(dest => dest.UserName, src => src.UserName)
                .Map(dest => dest.Name, src =>
                    src.Student != null ? src.Student.Name :
                    src.Individual != null ? src.Individual.Name :
                    src.Admin != null ? src.Admin.Name : 
                    null)
                .Map(dest => dest.LastName, src =>
                    src.Student != null ? src.Student.LastName :
                    src.Individual != null ? src.Individual.LastName :
                    src.Admin != null ? src.Admin.LastName :
                    null)
                .Map(dest => dest.CompanyName, src => src.Company != null ? src.Company.CompanyName : null)
                .Map(dest => dest.LegalName, src => src.Company != null ? src.Company.LegalName : null)
                .Map(dest => dest.Rating, src =>
                    src.Student != null ? src.Student.Rating :
                    src.Individual != null ? src.Individual.Rating :
                    src.Company != null ? src.Company.Rating :
                    (float?)null)
                .Map(dest => dest.CurriculumVitae, src => src.Student != null ? src.Student.CurriculumVitae : null)
                .Map(dest => dest.Rut, src => src.Rut)
                .Map(dest => dest.Email, src => src.Email)
                .Map(dest => dest.PhoneNumber, src => src.PhoneNumber)
                .Map(dest => dest.AboutMe, src => src.AboutMe);
        }
    }
}