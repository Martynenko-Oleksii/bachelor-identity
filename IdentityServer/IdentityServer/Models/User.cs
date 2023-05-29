using Microsoft.AspNetCore.Identity;

namespace IdentityServer.Models
{
    public class User : IdentityUser<Guid>
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? LastSignInTime { get; set; }
        public bool UserConfirmed { get; set; } = false;
    }
}
