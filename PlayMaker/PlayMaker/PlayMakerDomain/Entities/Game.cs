namespace PlayMakerDomain.Entities
{
    public class Game
    {
        public int Id { get; set; }
        public DateTime CreationDate { get; set; } = DateTime.Now;
        public DateTime? GameDate { get; set; } = null;
        public DateTime? GameTime { get; set; } = null;
        public virtual SportType Sport { get; set; }
        public int SportId { get; set; }
        public virtual IEnumerable<Player> Players { get; set; }
    }
}
