using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;
using Task9Identity.Models;
using Task9Identity.Models.VMs;

namespace Task9Identity.Services.IServices
{
    public interface IAccountService
    {
        public Task<string> RegisterNewUser(RegisterVM registerVM);
        public Task<bool> Login(LoginVM loginVM);
        public Task Logout();
        public Task<string> ResetPassword(string userName, ResetPasswrodVM resetVM);


        public List<IdentityRole> GetRoles();
        public Task<string> CreateRole(CreateRoleVM createRoleVM);
        public IdentityRole GetRoleById(string roleId);
        public Task<IList<ApplicationUser>> GetUsersInRole(IdentityRole identityRole);
        public Task<IList<ApplicationUser>> GetUsersNotInRole(IdentityRole identityRole);
        public IList<ApplicationUser> GetAllUsers();
        public Task<bool> AddUserToRole(string userId, string roleId);
        public Task<bool> RemoveUserFromRole(string userId, string roleId);
        public Task<string> DeleteRole(string roleId);
        public Task<string> GeneratePasswordResetToken(string email);
        public Task<string> ForgotRestPassword(ForgetPasswordSetNewVM vm);
    }
}
