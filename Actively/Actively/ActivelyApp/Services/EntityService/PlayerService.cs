using ActivelyApp.CustomExceptions;
using ActivelyApp.Models.EntityDto;
using ActivelyDomain.Entities;
using ActivelyInfrastructure.Repositories.EntityRepositories.PlayerRepository;
using AutoMapper;
using Resources;

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
        public async Task<IEnumerable<PlayerDto>> GetAll()
        {        
            var players = await _playerRepository.GetAll() ?? new List<Player>();
            var playersDto = _mapper.Map<IEnumerable<PlayerDto>>(players);
            return playersDto;
        }

        public async Task<PlayerDto> GetById(int id)
        {
            var player = await _playerRepository.GetById(id) ??
                throw new NotFoundEntityException(Common.PlayerNotExistsError);
            var playerDto = _mapper.Map<PlayerDto>(player);
            return playerDto;
        }

        public async Task Delete(int id)
        {
            var playerToDelete = await _playerRepository.GetById(id);
            if (playerToDelete != null)
            {
                await _playerRepository.Delete(playerToDelete);
                await _playerRepository.Save();
            }

            else
                throw new NotFoundEntityException(Common.PlayerNotExistsError);

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
                throw new NotFoundEntityException(Common.PlayerNotExistsError);

            await _playerRepository.Update(playerToUpdate);
            await _playerRepository.Save();
        }

        public async Task Create(CreatePlayerInfo newPlayerInfo)
        {
            var newPlayer = _mapper.Map<Player>(newPlayerInfo);
            await _playerRepository.Create(newPlayer);
            await _playerRepository.Save();

        }
    }
}
