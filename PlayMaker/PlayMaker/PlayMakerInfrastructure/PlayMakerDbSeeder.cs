using PlayMakerDomain.Entities;

namespace PlayMakerInfrastructure
{
    public class PlayMakerDbSeeder
    {
        private readonly PlayMakerDbContext _dbContext;

        public PlayMakerDbSeeder(PlayMakerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Seed()
        {
            if (_dbContext.Database.CanConnect())
            {              
                if (!_dbContext.SportType.Any())
                {
                    var types = CreateSportTypes();
                    _dbContext.SportType.AddRange(types);
                    _dbContext.SaveChanges();
                }

                if (!_dbContext.Game.Any())
                {
                    var games = CreateGames();
                    _dbContext.Game.AddRange(games);
                    _dbContext.SaveChanges();
                }

            }
        }   

        private List<SportType> CreateSportTypes()
        {
            var types = new List<SportType>()
            {
                new SportType() { Name = "football" },
                new SportType() { Name = "basketball" },
                new SportType() { Name = "volleyball" },
            };
            return types;
        }      
        private List<Game> CreateGames()
        {
            var games = new List<Game>()
           {
               new Game()
               {
                 GameDate = DateTime.Now,
                 SportId = 1,
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
                 GameDate = DateTime.Now,
                 SportId = 1,
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
