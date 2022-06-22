using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;
using Task9Identity.Data;
using Task9Identity.Models;
using Task9Identity.Models.VMs;
using Task9Identity.Services.IServices;
using System.Linq;

namespace Task9Identity.Services
{
    public class AccountService : IAccountService
    {
        public readonly ApplicationDbContext _dbContext;
        UserManager<ApplicationUser> _userManager;
        SignInManager<ApplicationUser> _signInManager;
        RoleManager<IdentityRole> _roleManager;

        public AccountService(ApplicationDbContext db, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager
            , RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _dbContext = db;
        }

        public async Task<bool> Login(LoginVM vm)
        {
            var signInResult = await _signInManager.PasswordSignInAsync(vm.Email, vm.Password, vm.RememberMe, false);
            if (signInResult.Succeeded)
            {
                return true;
            }
            return false;
        }

        public async Task Logout()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<string> RegisterNewUser(RegisterVM registerVM)
        {
            var _user = new ApplicationUser
            {
                Email = registerVM.Email,
                FullName = registerVM.Name,
                UserName = registerVM.Email
            };
            var result = _userManager.CreateAsync(_user, registerVM.Password).Result;
            if (result.Succeeded)
            {
                await check(_user.Id);
                await _signInManager.SignInAsync(_user, isPersistent: false);
            }
            else
            {
                string output = string.Empty;
                foreach (var item in result.Errors)
                {
                    output = output + item.Description;
                }
                return output;
            }
            return null;
        }
        public async Task check(string userId)
        {
            var role = await _roleManager.FindByNameAsync("Admin");
            if (role == null)
            {
                var user =await _userManager.FindByIdAsync(userId);
                await _userManager.AddToRoleAsync(user, "Admin");
            }
            
        }
        public async Task<string> ResetPassword(string userName,ResetPasswrodVM resetVM)
        {
            ApplicationUser applicationUser = await _userManager.FindByNameAsync(userName);
            var user = applicationUser;
            var result = await _userManager.ChangePasswordAsync(user, resetVM.CurrentPasswrod, resetVM.Password);
            if (result.Succeeded)
            {
                await _signInManager.SignOutAsync();
            }
            else
            {
                string output = string.Empty;
                foreach (var item in result.Errors)
                {
                    output = output + item.Description;
                }
                return output;
            }
            return null;
        }

        public List<IdentityRole> GetRoles()
        {
            return _roleManager.Roles.ToList();
        }

        public async Task<string> CreateRole(CreateRoleVM createRoleVM)
        {
            var result = await _roleManager.CreateAsync(new IdentityRole(createRoleVM.RoleName));
            if (result.Succeeded)
            {
                await _signInManager.SignOutAsync();
            }
            else
            {
                string output = string.Empty;
                foreach (var item in result.Errors)
                {
                    output = output + item.Description;
                }
                return output;
            }
            return null;
        }

        public IdentityRole GetRoleById(string roleId)
        {
            return _roleManager.Roles.Where(a=>a.Id==roleId).FirstOrDefault();
        }

        public async Task<IList<ApplicationUser>> GetUsersInRole(IdentityRole identityRole)
        {
            return await _userManager.GetUsersInRoleAsync(identityRole.Name);
        }

        public IList<ApplicationUser> GetAllUsers()
        {
            return  _userManager.Users.ToList();
        }
        public async Task<IList<ApplicationUser>> GetUsersNotInRole(IdentityRole identityRole)
        {
            List<ApplicationUser> usersInRole = _userManager.GetUsersInRoleAsync(identityRole.Name).GetAwaiter().GetResult().ToList();
            List<ApplicationUser> allUsers = _userManager.Users.ToList();
            List<ApplicationUser> remainingUsers =allUsers.Where(a=>usersInRole.Select(b=>b.Id).ToList().Contains(a.Id)).ToList();
            return remainingUsers;
        }
        public async Task<bool> AddUserToRole(string userId, string roleId)
        {
            var role =await _roleManager.FindByIdAsync(roleId);
            var user = await _userManager.FindByIdAsync(userId);
            var result = await _userManager.AddToRoleAsync(user, role.Name);
            if (result.Succeeded)
            {
                return true;
            }
            return false;
        }
        public async Task<bool> RemoveUserFromRole(string userId, string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            var user = await _userManager.FindByIdAsync(userId);
            var result = await _userManager.RemoveFromRoleAsync(user, role.Name);

            if (result.Succeeded)
            {
                return true;
            }
            return false;
        }

        public async Task<string> DeleteRole(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            var users = await _userManager.GetUsersInRoleAsync(role.Name);
            if (users.Count() > 0)
                return "This Role Contains Users";
            await _roleManager.DeleteAsync(role);
            return null;
        }

        public async Task<string> GeneratePasswordResetToken(string email)
        {
            var user = await _userManager.FindByNameAsync(email);
            if (user == null)
                return null;
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            return token;
        }

        public async Task<string> ForgotRestPassword(ForgetPasswordSetNewVM vm)
        {
            var user = await _userManager.FindByNameAsync(vm.Email);
            var result = await _userManager.ResetPasswordAsync(user,vm.Token,vm.Password);
            if (result.Succeeded)
            {
                return null;
            }
            else
            {
                string output = string.Empty;
                foreach (var item in result.Errors)
                {
                    output = output + item.Description;
                }
                return output;
            }
        }
    }
}
