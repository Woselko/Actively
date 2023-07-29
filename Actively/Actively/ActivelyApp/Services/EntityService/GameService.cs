using ActivelyApp.CustomExceptions;
using ActivelyApp.Models.EntityDto;
using ActivelyDomain.Entities;
using ActivelyInfrastructure.Repositories.EntityRepositories.GameRepository;
using AutoMapper;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
            IEnumerable<Game> games = null;
            try
            {
                games = await _gameRepository.GetAll() ?? new List<Game>();
            }
            catch (Exception)
            {
                //log
            }
            var gamesDto = _mapper.Map<IEnumerable<GameDto>>(games);
            return gamesDto;           
        }

        public async Task<GameDto?> GetById(int id)
        {
            Game game = null;
            try
            {
                game = await _gameRepository.GetById(id);
            }
            catch (Exception)
            {
                //log
            }
            if (game == null)
                return null;
            var gameDto = _mapper.Map<GameDto>(game);
            return gameDto;
        }

        public async Task Delete(int id)
        {
            Game gameToDelete = null;
            try
            {
                gameToDelete = await _gameRepository.GetById(id);
                if (gameToDelete == null)
                {
                    throw new NotFoundEntityException(Common.GameNotExistsError);
                }
                await _gameRepository.Delete(id);
                await _gameRepository.Save();
            }
            catch (NotFoundEntityException)
            {
                //log
            }
            catch (Exception)
            {
                //log
            }
           
        }

        public async Task Update(UpdateGameInfo game, int id)
        {
            Game gameToUpdate = null;
            try
            {
                gameToUpdate = await _gameRepository.GetById(id);
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
            catch (Exception)
            {
                //log
            }
          
            

        }

        public async Task Create(CreateGameInfo newGame)
        {
            try
            {
                if (newGame != null)
                {
                    Game game = _mapper.Map<Game>(newGame);
                    await _gameRepository.Create(game);
                    await _gameRepository.Save();

                }
            }
            catch (Exception)
            {
                //log
            }
         
        }
    }
}
