using ActivelyApp.CustomExceptions;
using ActivelyApp.Models.EntityDto;
using ActivelyDomain.Entities;
using ActivelyInfrastructure.Repositories.EntityRepositories.GameRepository;
using AutoMapper;
using Resources;

namespace ActivelyApp.Services.EntityService
{

    public class GameService : IGameService
    {
        private readonly IGameRepository _gameRepository;
        private readonly IMapper _mapper;

        public GameService(IGameRepository gameRepository, IMapper mapper)
        {
            _gameRepository = gameRepository;
            _mapper = mapper;
        }
        public async Task<IEnumerable<GameDto>> GetAll()
        {
            var games = await _gameRepository.GetAll() ?? new List<Game>();
            var gamesDto = _mapper.Map<IEnumerable<GameDto>>(games);
            return gamesDto;
            
        }

        public async Task<GameDto> GetById(int id)
        {
            var game = await _gameRepository.GetById(id) ??
                throw new NotFoundEntityException(Common.GameNotExistsError);
            var gameDto = _mapper.Map<GameDto>(game);
            return gameDto;
        }

        public async Task Delete(int id)
        {
            var gameToDelete = await _gameRepository.GetById(id);
            if (gameToDelete == null)
            {
                throw new NotFoundEntityException(Common.GameNotExistsError);
            }      
            await _gameRepository.Delete(id);
            await _gameRepository.Save();
        }

        public async Task Update(UpdateGameInfo game, int id)
        {
            var gameToUpdate = await _gameRepository.GetById(id);
            if (gameToUpdate == null)
            {
                throw new NotFoundEntityException(Common.GameNotExistsError);
            }
            else
            {
                gameToUpdate.GameTime = game.GameTime;
                await _gameRepository.Update(gameToUpdate);
            }
            await _gameRepository.Save();

        }

        public async Task Create(CreateGameInfo newGame)
        {
            Game game = _mapper.Map<Game>(newGame);
            await _gameRepository.Create(game);
            await _gameRepository.Save();

        }
    }
}
