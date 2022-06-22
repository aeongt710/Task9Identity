using System.ComponentModel.DataAnnotations;

namespace Task9Identity.Models.VMs
{
    public class CreateRoleVM
    {
        [Required]
        [Display(Name ="Role Name")]
        public string RoleName { get; set; }
    }
}
