using AutoMapper;
using WebhookRelayService.Models;

namespace WebhookRelayService.MappingProfiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<WebhookUserCreateDTO, WebhookUser>();
        }
    }
}
