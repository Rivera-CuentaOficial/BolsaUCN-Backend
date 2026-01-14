using bolsafeucn_back.src.Application.DTOs.UserDTOs;
using bolsafeucn_back.src.Application.DTOs.UserDTOs.AdminDTOs;
using bolsafeucn_back.src.Application.DTOs.UserDTOs.UserProfileDTOs;
using bolsafeucn_back.src.Domain.Models;
using Mapster;

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
            ConfigureGetProfileMapping();
            ConfigureUpdateMappings();
            ConfigureAdminMappings();
        }

        public void ConfigureGetProfileMapping()
        {
            TypeAdapterConfig<User, GetUserProfileDTO>
                .NewConfig()
                .Map(dest => dest.UserName, src => src.UserName)
                .Map(dest => dest.FirstName, src => src.FirstName)
                .Map(dest => dest.LastName, src => src.LastName)
                .Map(dest => dest.Rut, src => src.Rut)
                .Map(dest => dest.Email, src => src.Email)
                .Map(dest => dest.PhoneNumber, src => src.PhoneNumber)
                .Map(dest => dest.AboutMe, src => src.AboutMe)
                .Map(dest => dest.Rating, src => src.Rating)
                .Map(dest => dest.CurriculumVitae, src => src.CV != null ? src.CV.Url : null)
                .Map(
                    dest => dest.Disability,
                    src => src.Disability != null ? src.Disability.ToString() : null
                )
                .Map(dest => dest.ProfilePhoto, src => src.ProfilePhoto!.Url);
            TypeAdapterConfig<User, GetPhotoDTO>
                .NewConfig()
                .Map(
                    dest => dest.PhotoUrl,
                    src => src.ProfilePhoto != null ? src.ProfilePhoto.Url : string.Empty
                );
            TypeAdapterConfig<User, GetCVDTO>
                .NewConfig()
                .Map(dest => dest.Url, src => src.CV!.Url)
                .Map(dest => dest.OriginalFileName, src => src.CV!.OriginalFileName)
                .Map(dest => dest.FileSizeBytes, src => src.CV!.FileSizeBytes)
                .Map(dest => dest.UploadDate, src => src.CV!.CreatedAt);
        }

        public void ConfigureUpdateProfileMappings()
        {
            TypeAdapterConfig<UpdateUserProfileDTO, User>
                .NewConfig()
                .IgnoreNullValues(true)
                .Map(dest => dest.UserName, src => src.UserName)
                .Map(dest => dest.FirstName, src => src.FirstName)
                .Map(dest => dest.LastName, src => src.LastName)
                .Map(dest => dest.Email, src => src.Email)
                .Map(dest => dest.AboutMe, src => src.AboutMe)
                .Map(dest => dest.PhoneNumber, src => src.PhoneNumber);
        }

        /// <summary>
        /// Configura el mapeo de GeneralUser a GetUserProfileDTO para ver el perfil de usuario.
        /// </summary>
        public void ConfigureViewProfileMapping()
        {
            TypeAdapterConfig<User, GetStudentProfileDTO>
                .NewConfig()
                .Map(dest => dest.UserName, src => src.UserName)
                .Map(dest => dest.Name, src => src.FirstName)
                .Map(dest => dest.LastName, src => src.LastName)
                //.Map(dest => dest.Rating, src => src.Student!.Rating)
                .Map(dest => dest.CurriculumVitae, src => src.CV != null ? src.CV.Url : null)
                .Map(dest => dest.Rut, src => src.Rut)
                .Map(dest => dest.Email, src => src.Email)
                .Map(dest => dest.PhoneNumber, src => src.PhoneNumber)
                .Map(dest => dest.AboutMe, src => src.AboutMe)
                .Map(dest => dest.ProfilePhoto, src => src.ProfilePhoto!.Url);

            TypeAdapterConfig<User, GetIndividualProfileDTO>
                .NewConfig()
                .Map(dest => dest.UserName, src => src.UserName)
                .Map(dest => dest.Name, src => src.FirstName)
                .Map(dest => dest.LastName, src => src.LastName)
                //.Map(dest => dest.Rating, src => src.Individual!.Rating)
                .Map(dest => dest.Rut, src => src.Rut)
                .Map(dest => dest.Email, src => src.Email)
                .Map(dest => dest.PhoneNumber, src => src.PhoneNumber)
                .Map(dest => dest.AboutMe, src => src.AboutMe)
                .Map(dest => dest.ProfilePhoto, src => src.ProfilePhoto!.Url);

            TypeAdapterConfig<User, GetCompanyProfileDTO>
                .NewConfig()
                .Map(dest => dest.UserName, src => src.UserName)
                .Map(dest => dest.CompanyName, src => src.FirstName)
                .Map(dest => dest.LegalName, src => src.LastName)
                //.Map(dest => dest.Rating, src => src.Company!.Rating)
                .Map(dest => dest.Rut, src => src.Rut)
                .Map(dest => dest.Email, src => src.Email)
                .Map(dest => dest.PhoneNumber, src => src.PhoneNumber)
                .Map(dest => dest.AboutMe, src => src.AboutMe)
                .Map(dest => dest.ProfilePhoto, src => src.ProfilePhoto!.Url);

            TypeAdapterConfig<User, GetAdminProfileDTO>
                .NewConfig()
                .Map(dest => dest.UserName, src => src.UserName)
                .Map(dest => dest.Name, src => src.FirstName)
                .Map(dest => dest.LastName, src => src.LastName)
                .Map(dest => dest.Rut, src => src.Rut)
                .Map(dest => dest.Email, src => src.Email)
                .Map(dest => dest.PhoneNumber, src => src.PhoneNumber)
                .Map(dest => dest.AboutMe, src => src.AboutMe)
                .Map(dest => dest.ProfilePhoto, src => src.ProfilePhoto!.Url);
        }

        /// <summary>
        /// Configura el mapeo de UpdateParamsDTO a GeneralUser para actualizar el perfil de usuario.
        /// </summary>
        public void ConfigureUpdateMappings()
        {
            TypeAdapterConfig<UpdateStudentParamsDTO, User>
                .NewConfig()
                .IgnoreNullValues(true)
                .Ignore(src => src.ProfilePhoto!)
                .Map(dest => dest.UserName, src => src.UserName)
                .Map(dest => dest.FirstName, src => src.Name)
                .Map(dest => dest.LastName, src => src.LastName)
                .Map(dest => dest.Email, src => src.Email)
                .Map(dest => dest.AboutMe, src => src.AboutMe)
                .Map(dest => dest.PhoneNumber, src => src.PhoneNumber);

            TypeAdapterConfig<UpdateIndividualParamsDTO, User>
                .NewConfig()
                .IgnoreNullValues(true)
                .Map(dest => dest.UserName, src => src.UserName)
                .Map(dest => dest.FirstName, src => src.Name)
                .Map(dest => dest.LastName, src => src.LastName)
                .Map(dest => dest.Email, src => src.Email)
                .Map(dest => dest.AboutMe, src => src.AboutMe)
                .Map(dest => dest.PhoneNumber, src => src.PhoneNumber);

            TypeAdapterConfig<UpdateCompanyParamsDTO, User>
                .NewConfig()
                .IgnoreNullValues(true)
                .Map(dest => dest.UserName, src => src.UserName)
                .Map(dest => dest.FirstName, src => src.FirstName)
                .Map(dest => dest.LastName, src => src.LastName)
                .Map(dest => dest.Email, src => src.Email)
                .Map(dest => dest.AboutMe, src => src.AboutMe)
                .Map(dest => dest.PhoneNumber, src => src.PhoneNumber);

            TypeAdapterConfig<UpdateAdminParamsDTO, User>
                .NewConfig()
                .IgnoreNullValues(true)
                .Map(dest => dest.UserName, src => src.UserName)
                .Map(dest => dest.FirstName, src => src.Name)
                .Map(dest => dest.LastName, src => src.LastName)
                .Map(dest => dest.Email, src => src.Email)
                .Map(dest => dest.AboutMe, src => src.AboutMe)
                .Map(dest => dest.PhoneNumber, src => src.PhoneNumber);
        }

        public void ConfigureAdminMappings()
        {
            TypeAdapterConfig<User, UserForAdminDTO>
                .NewConfig()
                .Map(dest => dest.UserName, src => src.UserName)
                .Map(dest => dest.Email, src => src.Email)
                .Map(dest => dest.Rut, src => src.Rut)
                .Map(dest => dest.Rating, src => src.Rating)
                .Map(dest => dest.UserType, src => src.UserType.ToString())
                .Map(dest => dest.Banned, src => src.Banned);

            TypeAdapterConfig<User, UserProfileForAdminDTO>
                .NewConfig()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.Username, src => src.UserName)
                .Map(dest => dest.Email, src => src.Email)
                .Map(dest => dest.FirstName, src => src.FirstName)
                .Map(dest => dest.LastName, src => src.LastName)
                .Map(dest => dest.PhoneNumber, src => src.PhoneNumber)
                .Map(dest => dest.Rut, src => src.Rut)
                .Map(dest => dest.Rating, src => src.Rating)
                .Map(
                    dest => dest.ProfilePictureUrl,
                    src => src.ProfilePhoto != null ? src.ProfilePhoto.Url : null
                )
                .Map(dest => dest.AboutMe, src => src.AboutMe)
                .Map(dest => dest.UserType, src => src.UserType.ToString())
                .Map(dest => dest.Banned, src => src.Banned)
                .Map(dest => dest.CreatedAt, src => src.CreatedAt)
                .Map(dest => dest.UpdatedAt, src => src.UpdatedAt)
                .Map(dest => dest.LastLoginAt, src => src.LastLoginAt)
                .Map(dest => dest.CVUrl, src => src.CV != null ? src.CV.Url : null)
                .Map(
                    dest => dest.Disability,
                    src => src.Disability != null ? src.Disability.ToString() : null
                );
        }
    }
}
