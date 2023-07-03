namespace PlayMakerDomain.Entities
{
    public class Player
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NickName { get; set; }
        public virtual IEnumerable<Game> Games { get; set; }
       
    }
}
