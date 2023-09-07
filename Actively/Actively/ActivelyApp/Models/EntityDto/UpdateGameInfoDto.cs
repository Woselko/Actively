using ActivelyDomain.Entities;

namespace ActivelyApp.Models.EntityDto
{
    public class UpdateGameInfoDto
    {
        public DateTime? GameTime { get; set; } = DateTime.Today;
    }
}
