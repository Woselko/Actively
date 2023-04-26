using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayMakerDomain.Entities
{
    public class Game
    {
        public int Id { get; set; }
        public DateTime CreationDate { get; set; } = DateTime.Now;
        public DateTime GameDate { get; set; } 
        public virtual SportType Sport { get; set; }
        public int SportId { get; set; }
        public bool IsPayed { get; set; }
        public virtual IEnumerable<Player> Players { get; set; }
    }
}
