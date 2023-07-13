using ActivelyApp.CustomExceptions;
using ActivelyApp.Models.Player;
using ActivelyDomain.Entities;
using ActivelyInfrastructure.Repositories;
using AutoMapper;

namespace ActivelyApp.Services.EntityService
{
    public class PlayerService : IPlayerService
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly IMapper _mapper;

        public PlayerService(IPlayerRepository playerRepository, IMapper mapper)
        {
            _playerRepository = playerRepository;
            _mapper = mapper;
        }
        public async Task<IEnumerable<Player>> GetAll()
        {
            return await _playerRepository.GetAll();
        }

        public async Task<Player> GetById(int id)
        {
            return await _playerRepository.GetById(id) ?? 
                throw new NotFoundEntityException("Player does not exist");
        }

        public async Task Delete(int id)
        {
            var playerToDelete = await _playerRepository.GetById(id);
            if (playerToDelete != null)
                await _playerRepository.Remove(playerToDelete);
            else
                throw new NotFoundEntityException("Player does not exist");

        }

        public async Task Update(UpdatePlayerInfo updatePlayerInfo)
        {
            var updatedPlayer = _mapper.Map<Player>(updatePlayerInfo);
            await _playerRepository.Update(updatedPlayer);
        }

        public async Task Create(CreatePlayerInfo newPlayerInfo)
        {
            var newPlayer = _mapper.Map<Player>(newPlayerInfo);
            await _playerRepository.Add(newPlayer);

        }
    }
}
