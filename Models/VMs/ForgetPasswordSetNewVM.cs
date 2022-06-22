using System.ComponentModel.DataAnnotations;

namespace Task9Identity.Models.VMs
{
    public class ForgetPasswordSetNewVM
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Token { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "Enter strong password!", MinimumLength = 6)]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "Password Does Not Match!")]
        public string ConfirmPassword { get; set; }
    }
}
