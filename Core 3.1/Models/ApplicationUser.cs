using Microsoft.AspNetCore.Identity;

namespace Core_3._1.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Alias { get; set; }
    }
}
