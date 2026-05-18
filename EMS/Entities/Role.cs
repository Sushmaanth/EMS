namespace Entities
{
    public class Role
    {
        public int Id { get; set; }

        public string RoleName { get; set; } = "User";

        public ICollection<User> Users { get; set; }
    }
}