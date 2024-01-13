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
            IEnumerable<Player> players = null;
            try
            {
                players = await _playerRepository.GetAll() ?? new List<Player>();
            }
            catch (Exception)
            {
                //log
            }

            var playersDto = _mapper.Map<IEnumerable<PlayerDto>>(players);
            return playersDto;
        }

        public async Task<PlayerDto> GetById(int id)
        {
            Player player = null;
            try
            {
                player = await _playerRepository.GetById(id);
            }
            catch (Exception)
            {
               // log
            }

            if (player == null)
            {
                return null;
            }

            var playerDto = _mapper.Map<PlayerDto>(player);
            return playerDto;
        }

        public async Task Delete(int id)
        {
            Player playerToDelete = null;
            try
            {
                playerToDelete = await _playerRepository.GetById(id);
                if (playerToDelete != null)
                {
                    await _playerRepository.Delete(playerToDelete);
                    await _playerRepository.Save();
                }
                else
                    throw new NotFoundEntityException(Common.PlayerNotExists);
            }
            catch (NotFoundEntityException)
            {
                //log
            }
            catch (Exception)
            {
                // log
            }
        }
        public async Task Update(UpdatePlayerInfoDto updatePlayerInfo, int id)
        {
            Player playerToUpdate = null;
            try
            {
                playerToUpdate = await _playerRepository.GetById(id);
                if (playerToUpdate != null)
                {
                    playerToUpdate.LastName = updatePlayerInfo.LastName;
                    playerToUpdate.NickName = updatePlayerInfo.NickName;
                    await _playerRepository.Update(playerToUpdate);
                    await _playerRepository.Save();
                }
                else
                    throw new NotFoundEntityException(Common.PlayerNotExists);
            }
            catch (NotFoundEntityException)
            {
                //log
               
            }
            catch(Exception) 
            {
                //log
            }
        }
        public async Task Create(CreatePlayerInfoDto newPlayerInfo)
        {
            try
            {
                if (newPlayerInfo != null)
                {
                    var newPlayer = _mapper.Map<Player>(newPlayerInfo);
                    await _playerRepository.Create(newPlayer);
                    await _playerRepository.Save();
                }               
            }
            catch (Exception)
            {
                //log
            }           
        }
    }
}
