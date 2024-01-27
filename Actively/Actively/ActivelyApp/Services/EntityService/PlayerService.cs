using ActivelyApp.Models.ServiceModels;
using ActivelyDomain.Entities;
using ActivelyInfrastructure.Repositories.EntityRepositories.PlayerRepository;
using Resources;

namespace ActivelyApp.Services.EntityService
{
    public class PlayerService : IPlayerService
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly ILogger _logger;

        public PlayerService(IPlayerRepository playerRepository, ILogger logger)
        {
            _playerRepository = playerRepository;
            _logger = logger;
        }
        public async Task<ServiceResult<IEnumerable<Player>>> GetAllPlayers()
        {
            try
            {
                var players = await _playerRepository.GetAll() ?? Enumerable.Empty<Player>();

                if (!players.Any())
                {
                    return ServiceResult<IEnumerable<Player>>.Success(Common.PlayerNotExists, players);
                }

                return ServiceResult<IEnumerable<Player>>.Success(Common.Success, players);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return ServiceResult<IEnumerable<Player>>.Failure(Common.SomethingWentWrong);
            }

        }

        public async Task<ServiceResult<Player>> GetPlayerById(int id)
        {
            try
            {
                var player = await _playerRepository.GetById(id);
                if (player == null)
                    return ServiceResult<Player>.Failure(Common.PlayerNotExists);
                return ServiceResult<Player>.Success(Common.Success, player);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return ServiceResult<Player>.Failure(Common.SomethingWentWrong);
            }

        }

        public async Task<ServiceResult<Player>> DeletePlayer(int id)
        {
            try
            {
                var playerToDelete = await _playerRepository.GetById(id);
                if (playerToDelete is not null)
                {
                    await _playerRepository.Delete(playerToDelete);
                    await _playerRepository.Save();
                    return ServiceResult<Player>.Success(Common.SuccessfullyDeleted);
                }
                else
                {
                    return ServiceResult<Player>.Failure(Common.PlayerNotExists);
                }

            }

            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return ServiceResult<Player>.Failure(Common.SomethingWentWrong);
            }
        }
        public async Task<ServiceResult<Player>> UpdatePlayer(Player player, int id)
        {
            try
            {
                var playerToUpdate = await _playerRepository.GetById(id);
                if (playerToUpdate is not null)
                {
                    playerToUpdate.LastName = player.LastName;
                    await _playerRepository.Update(playerToUpdate);
                    await _playerRepository.Save();
                    return ServiceResult<Player>.Success(Common.SuccessfullyUpdated);
                }
                else
                {
                    return ServiceResult<Player>.Failure(Common.PlayerNotExists);
                }

            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return ServiceResult<Player>.Failure(Common.SomethingWentWrong);
            }
        }

        public async Task<ServiceResult<Player>> CreatePlayer(Player newPlayer)
        {
            try
            {
                await _playerRepository.Create(newPlayer);
                await _playerRepository.Save();
                return ServiceResult<Player>.Success(Common.SuccessfullyCreated);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return ServiceResult<Player>.Failure(Common.SomethingWentWrong);
            }
        }
    
    }
}


