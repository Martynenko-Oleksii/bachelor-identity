namespace IdentityServer.Models
{
    public class Role
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class NewUserDto
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Role[] Roles { get; set; }
    }
}
