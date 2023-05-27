using Microsoft.AspNetCore.Identity;

namespace IdentityServer.Models
{
    public class User : IdentityUser<Guid>
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? JobTitle { get; set; }
        public string? Address { get; set; }
        public string? Address2 { get; set; }
        public string? City { get; set; }
        public string? ZipCode { get; set; }
        public string? DepartmentName { get; set; }
        public string? DepartmentType { get; set; }
        public string? OrganizationName { get; set; }
        public string? OrganizationCity { get; set; }
    }
}
