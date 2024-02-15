using AutoMapper;
using WebhookRelayService.Models;
using WebhookRelayService.Repositories;

namespace WebhookRelayService.Services
{
    public interface IWebhookUserService
    {
        public Task<Guid> Create(WebhookUserCreateDTO dto);
        public Task Delete(Guid id);
        public Task<int> GetRegisteredUsersCount();
    }

    public class WebhookUserService : IWebhookUserService
    {
        private readonly IWebhookUserRepository _repository;
        private readonly IMapper _mapper;
        private readonly IHttpService _httpService;

        public WebhookUserService(IWebhookUserRepository repository, IMapper mapper, IHttpService httpService)
        {
            _repository = repository;
            _mapper = mapper;
            _httpService = httpService;
        }   

        public async Task<Guid> Create(WebhookUserCreateDTO dto)
        {
            var user = _mapper.Map<WebhookUser>(dto);
            if (await CheckEndpoint(user.NotificationEndpoint))
            {
                user = await _repository.Create(user);
            } 
            else
            {
                throw new InvalidDataException("Invalid UP endpoint");
            }
            return user.Id;
        }

        public async Task Delete(Guid id)
        {
            var user = await _repository.GetById(id);
            await _repository.Delete(user);
        }

        public async Task<int> GetRegisteredUsersCount()
        {
            return await _repository.Count();
        }

        private async Task<bool> CheckEndpoint(string endpoint)
        {
            var httpResponse = await _httpService.SendGet(endpoint);
            var responseContent = await httpResponse.Content.ReadAsStringAsync();
            return responseContent.Contains("{\"unifiedpush\":{\"version\":1}}");
        }
    }
}
