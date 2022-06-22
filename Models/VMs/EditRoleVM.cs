using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Task9Identity.Models.VMs
{
    public class EditRoleVM
    {
        public IdentityRole Role { get; set; }
        public IList<ApplicationUser> AllUsers { get; set; }
        public IList<ApplicationUser> UsersInRole { get; set; }
    }
}
