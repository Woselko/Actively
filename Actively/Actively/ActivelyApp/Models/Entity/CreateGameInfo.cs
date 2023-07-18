using ActivelyDomain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace ActivelyApp.Models.Entity
{
    public class CreateGameInfo
    {
        public DateTime? GameDate { get; set; } = DateTime.Now;
        public DateTime? GameTime { get; set; } = DateTime.Now;
        public SportType Sport { get; set; } = SportType.Football;

    }
}
