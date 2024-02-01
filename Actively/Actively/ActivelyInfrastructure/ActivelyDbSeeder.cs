using ActivelyDomain.Entities;
using System;

namespace ActivelyInfrastructure
{
    public class ActivelyDbSeeder
    {
        private readonly ActivelyDbContext _dbContext;

        public ActivelyDbSeeder(ActivelyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Seed()
        {
            if (_dbContext.Database.CanConnect())
            {     
                if (!_dbContext.Game.Any())
                {
                    var games = CreateGames();
                    _dbContext.Game.AddRange(games);
                    _dbContext.SaveChanges();
                }
            }
        }

        private List<Game> CreateGames()
        {
            var games = new List<Game>()
            {
               new Game()
               {
                 GameTime = DateTime.Now,
                 Sport = SportType.Football,
                 Players = new List<Player>()
                 {
                        new Player()
                        {
                            FirstName = "Emil",
                            LastName = "Bulkownik",
                            NickName = "Bulczo"
                        },
                         new Player()
                         {
                            FirstName = "Julie",
                            LastName = "Wojans",
                            NickName = "pączek",
                         },
                     new Player()
                     {
                            FirstName = "Marcin",
                            LastName = "Kapusta",
                            NickName = "Kaputa",
                     },
                 },
               },

               new Game()
               {
                 GameTime = DateTime.Now,
                 Sport = SportType.Football,
                 Players = new List<Player>()
                 {
                     new Player()
                     {
                         FirstName = "Karol",
                         LastName = "Wojtyla",
                         NickName = "Pablo",
                     },
                     new Player()
                     {
                         FirstName = "Pablo",
                         LastName = "Escobar",
                         NickName = "narcos",

                     },
                     new Player()
                     {
                         FirstName = "Witold",
                         LastName = "Ukasik",
                         NickName = "Wicio",
                     },

                 },

               }
           };

            return games;
        }

    }
}
