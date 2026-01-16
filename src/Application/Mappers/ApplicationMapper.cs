using bolsafeucn_back.src.Application.DTOs.JobAplicationDTO.ApplicantsDTOs;
using bolsafeucn_back.src.Domain.Models;
using Mapster;

namespace bolsafeucn_back.src.Application.Mappers
{
    public class ApplicationMapper
    {
        public void ConfigureAllMappings()
        {
            ConfigureApplicationMapping();
        }

        public void ConfigureApplicationMapping()
        {
            TypeAdapterConfig<JobApplication, ApplicationForApplicantDTO>
                .NewConfig()
                .Map(dest => dest.OfferId, src => src.JobOfferId)
                .Map(dest => dest.OfferTitle, src => src.JobOffer.Title)
                .Map(dest => dest.Status, src => src.Status.ToString())
                .Map(dest => dest.CreatedAt, src => src.CreatedAt);
        }
    }
}
