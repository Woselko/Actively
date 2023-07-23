using ActivelyDomain.Entities;

namespace ActivelyApp.Models.EntityDto
{
    public class UpdateGameInfo
    {
        public DateTime? GameTime { get; set; } = DateTime.Today;
    }
}
