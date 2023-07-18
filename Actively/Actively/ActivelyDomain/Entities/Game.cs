namespace ActivelyDomain.Entities
{
    public class Game
    {
        public int Id { get; set; }
        public DateTime CreationDate { get; set; } = DateTime.Now;
        public DateTime? GameDate { get; set; } = null;
        public DateTime? GameTime { get; set; } = null;
        public SportType Sport { get; set; }
        public virtual IEnumerable<Player>? Players { get; set; }
    }
}
