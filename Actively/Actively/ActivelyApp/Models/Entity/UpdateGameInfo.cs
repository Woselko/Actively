using ActivelyDomain.Entities;

namespace ActivelyApp.Models.Entity
{
    public class UpdateGameInfo
    {
        public DateTime? GameDate { get; set; } = null;
        public DateTime? GameTime { get; set; } = DateTime.Today;
    }
}
