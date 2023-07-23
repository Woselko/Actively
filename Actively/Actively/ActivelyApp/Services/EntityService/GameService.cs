using ActivelyApp.CustomExceptions;
using ActivelyApp.Models.EntityDto;
using ActivelyDomain.Entities;
using ActivelyInfrastructure.Repositories.EntityRepositories.GameRepository;
using AutoMapper;

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
        public async Task<IEnumerable<Game>> GetAll()
        {
            return await _gameRepository.GetAll();
        }

        public async Task<Game> GetById(int id)
        {
            return await _gameRepository.GetById(id) ??
                throw new NotFoundEntityException("Game does not exist");
        }

        public async Task Delete(int id)
        {
            var gameToDelete = await _gameRepository.GetById(id);
            if (gameToDelete == null)
            {
                throw new NotFoundEntityException("Game does not exist");
            }      
            await _gameRepository.Delete(id);
            await _gameRepository.Save();
        }

        public async Task Update(UpdateGameInfo game, int id)
        {
            var gameToUpdate = await _gameRepository.GetById(id);
            if (gameToUpdate == null)
            {
                throw new NotFoundEntityException("Game does not exist");
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
