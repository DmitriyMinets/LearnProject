using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Rocky_Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
    }
}
