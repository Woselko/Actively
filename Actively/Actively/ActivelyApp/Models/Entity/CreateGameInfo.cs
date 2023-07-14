using ActivelyDomain.Entities;

namespace ActivelyApp.Models.Entity
{
    public class CreateGameInfo
    {
        public DateTime CreationDate { get; set; } = DateTime.Now;
        public DateTime? GameDate { get; set; } = null;
        public DateTime? GameTime { get; set; } = null;
        public IEnumerable<ActivelyDomain.Entities.Player> Players { get; set; }
        public SportType? Sport { get; set; }

    }
}
