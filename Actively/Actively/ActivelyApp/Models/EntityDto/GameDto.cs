using ActivelyDomain.Entities;

namespace ActivelyApp.Models.EntityDto
{
    public class GameDto
    {
        public DateTime CreationDate { get; set; } = DateTime.Now;
        public DateTime? GameTime { get; set; } = null;
        public SportType Sport { get; set; } 
        public IEnumerable<PlayerDto>? Players { get; set; }
    }
}
