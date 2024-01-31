using ActivelyDomain.Entities;

namespace ActivelyApp.Models.EntityDto
{
    public class PlayerDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NickName { get; set; }
        public IEnumerable<GameDto>? Games { get; set; }
    }
}
