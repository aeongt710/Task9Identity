using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Task9Identity.Models
{
    public class ApplicationUser:IdentityUser
    {
        [Required]
        public string FullName { get; set; }
    }
}
