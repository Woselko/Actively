using ActivelyApp.Models.ServiceModels;
using ActivelyDomain.Entities;
using ActivelyInfrastructure.Repositories.EntityRepositories.GameRepository;
using Resources;

namespace ActivelyApp.Services.EntityService
{

    public class GameService : IGameService
    {
        private readonly IGameRepository _gameRepository;
        private readonly ILogger<GameService> _logger;

        public GameService(IGameRepository gameRepository, ILogger<GameService> logger)
        {
            _gameRepository = gameRepository;
            _logger = logger;
        }
        public async Task<ServiceResult<IEnumerable<Game>>> GetAllGames()
        {
            try
            {
                var games = await _gameRepository.GetAll() ?? Enumerable.Empty<Game>();

                if (!games.Any())
                {
                    return ServiceResult<IEnumerable<Game>>.Success(Common.GameNotExists, games);
                }

                return ServiceResult<IEnumerable<Game>>.Success(Common.Success, games);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return ServiceResult<IEnumerable<Game>>.Failure(Common.SomethingWentWrong);
            }

        }

        public async Task<ServiceResult<Game>> GetGameById(int id)
        {
            try
            {
                var game = await _gameRepository.GetById(id);
                if (game == null)
                    return ServiceResult<Game>.Failure(Common.GameNotExists);
                return ServiceResult<Game>.Success(Common.Success, game);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return ServiceResult<Game>.Failure(Common.SomethingWentWrong);
            } 
        }

        public async Task<ServiceResult<Game>> DeleteGame(int id)
        {
            try
            {
                var gameToDelete = await _gameRepository.GetById(id);
                if (gameToDelete is not null)
                {
                    await _gameRepository.Delete(gameToDelete);
                    await _gameRepository.Save();
                    return ServiceResult<Game>.Success(Common.SuccessfullyDeleted);
                }
                else
                {
                    return ServiceResult<Game>.Failure(Common.GameNotExists);
                }
            }

            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return ServiceResult<Game>.Failure(Common.SomethingWentWrong);
            }
        }
        public async Task<ServiceResult<Game>> UpdateGame(Game game, int id)
        {
            try
            {
                var gameToUpdate = await _gameRepository.GetById(id);
                if (gameToUpdate is not null)
                {
                    gameToUpdate.GameTime = game.GameTime;
                    await _gameRepository.Update(gameToUpdate);
                    await _gameRepository.Save();
                    return ServiceResult<Game>.Success(Common.SuccessfullyUpdated);
                }
                else
                {
                    return ServiceResult<Game>.Failure(Common.GameNotExists);
                }

            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return ServiceResult<Game>.Failure(Common.SomethingWentWrong);
            }
        }

        public async Task<ServiceResult<Game>> CreateGame(Game newGame)
        {
            try
            {
                    await _gameRepository.Create(newGame);
                    await _gameRepository.Save();
                return ServiceResult<Game>.Success(Common.SuccessfullyCreated);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return ServiceResult<Game>.Failure(Common.SomethingWentWrong);
            }
        }
    }
}


