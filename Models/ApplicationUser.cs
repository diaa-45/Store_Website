using Microsoft.AspNetCore.Identity;

namespace STORE_Website.Models
{
    public class ApplicationUser: IdentityUser
    {
        public required string FullName { get; set; }
        public required string Address { get; set; }
        public required string City { get; set; }
        public string? PostalCode { get; set; }
        public List<Order>? Orders { get; set; }

    }
}
