using System.ComponentModel.DataAnnotations;

namespace Task9Identity.Models.VMs
{
    public class ForgetPasswordVM
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
