using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PlayMakerDomain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

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
                if (!_dbContext.Roles.Any())
                {
                    var roles = CreateRoles();
                    _dbContext.Roles.AddRange(roles);
                    _dbContext.SaveChanges();
                }

                if (!_dbContext.SportType.Any())
                {
                    var types = CreateSportTypes();
                    _dbContext.SportType.AddRange(types);
                    _dbContext.SaveChanges();
                }

                if (!_dbContext.Game.Any())
                {
                    var games = CreaateGames();
                    _dbContext.Game.AddRange(games);
                    _dbContext.SaveChanges();
                }
                if (!_dbContext.Payment.Any())
                {
                    var payments = CreatePayments();
                    _dbContext.Payment.AddRange(payments);
                    _dbContext.SaveChanges();
                }
            }
        }

        private List<IdentityRole> CreateRoles()
        {
            var roles = new List<IdentityRole>()
            {
                  new IdentityRole()
                  {
                      Name = "admin",
                      NormalizedName = "admin"
                  },
                  new IdentityRole()
                  {
                      Name = "player",
                      NormalizedName = "player"
                  }
            };
            return roles;
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

        private List<Payment> CreatePayments()
        {
            var payments = new List<Payment>()
            {
                new Payment()
                {
                    GameId = 1,
                    PlayerId = 1,
                    Amount = 20
                },
                new Payment()
                {
                    GameId = 1,
                    PlayerId = 2,
                    Amount = 20
                },
                new Payment()
                {
                    GameId = 1,
                    PlayerId = 3,
                    Amount = 20
                },
            };

            return payments;
        }
        private List<Game> CreaateGames()
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
                            NickName = "Bulczo",
                            Payments = new List<Payment>()
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
