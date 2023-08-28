using AutoMapper;
using WebhookRelayService.Models;
using WebhookRelayService.Repositories;

namespace WebhookRelayService.Services
{
    public interface IWebhookUserService
    {
        public Task<Guid> Create(WebhookUserCreateDTO dto);
        public Task Delete(Guid id);
    }

    public class WebhookUserService : IWebhookUserService
    {
        private readonly IWebhookUserRepository _repository;
        private readonly IMapper _mapper;

        public WebhookUserService(IWebhookUserRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }   

        public async Task<Guid> Create(WebhookUserCreateDTO dto)
        {
            var user = _mapper.Map<WebhookUser>(dto);
            user = await _repository.Create(user);
            return user.Id;
        }

        public async Task Delete(Guid id)
        {
            var user = await _repository.GetById(id);
            await _repository.Delete(user);
        }
    }
}
