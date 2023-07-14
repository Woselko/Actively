using ActivelyDomain.Entities;

namespace ActivelyApp.Models.Entity
{
    public class UpdateGameInfo
    {
        public DateTime? GameDate { get; set; } = null;
        public DateTime? GameTime { get; set; } = null;
        public IEnumerable<Player> Players { get; set; }
    }
}
