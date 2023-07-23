using ActivelyApp.CustomExceptions;
using ActivelyApp.Models.EntityDto;
using ActivelyDomain.Entities;
using ActivelyInfrastructure.Repositories.EntityRepositories.PlayerRepository;
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
            {
                await _playerRepository.Remove(playerToDelete);
                await _playerRepository.Save();
            }

            else
                throw new NotFoundEntityException("Player does not exist");

        }

        public async Task Update(UpdatePlayerInfo updatePlayerInfo, int id)
        {
            var playerToUpdate = await _playerRepository.GetById(id);
            if (playerToUpdate != null)
            {
                playerToUpdate.LastName = updatePlayerInfo.LastName;
                playerToUpdate.NickName = updatePlayerInfo.NickName;
            }
            else
                throw new NotFoundEntityException("Player does not exist");

            await _playerRepository.Update(playerToUpdate);
            await _playerRepository.Save();
        }

        public async Task Create(CreatePlayerInfo newPlayerInfo)
        {
            var newPlayer = _mapper.Map<Player>(newPlayerInfo);
            await _playerRepository.Add(newPlayer);
            await _playerRepository.Save();

        }
    }
}
