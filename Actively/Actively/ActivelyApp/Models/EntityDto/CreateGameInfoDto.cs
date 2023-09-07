using ActivelyDomain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace ActivelyApp.Models.EntityDto
{
    public class CreateGameInfoDto
    {
        public DateTime? GameDate { get; set; } = DateTime.Now;
        public DateTime? GameTime { get; set; } = DateTime.Now;
        public SportType Sport { get; set; } = SportType.Football;

    }
}
