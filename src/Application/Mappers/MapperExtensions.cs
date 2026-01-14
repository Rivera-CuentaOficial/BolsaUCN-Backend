using Mapster;

namespace bolsafeucn_back.src.Application.Mappers
{
    public class MapperExtensions
    {
        public static void ConfigureMapster(IServiceProvider serviceProvider)
        {
            var userMapper = serviceProvider.GetService<UserMapper>();
            userMapper?.ConfigureAllMappings();
            var offerMapper = serviceProvider.GetService<OfferMapper>();
            offerMapper?.ConfigureAllMappings();
            var buySellMapper = serviceProvider.GetService<BuySellMapper>();
            buySellMapper?.ConfigureAllMappings();
            var profileMapper = serviceProvider.GetService<ProfileMapper>();
            profileMapper?.ConfigureAllMappings();

            TypeAdapterConfig.GlobalSettings.Default.IgnoreNullValues(true);
        }
    }
}
