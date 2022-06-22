using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Task9Identity.Models.VMs;
using Task9Identity.Services.IServices;

namespace Task9Identity.Controllers
{
    [Authorize(Roles ="Admin")]
    public class RolesController : Controller
    {
        private readonly IAccountService _accountService;
        public RolesController(IAccountService accountService)
        {
            _accountService = accountService;
        }
        public IActionResult Index()
        {
            var roles = _accountService.GetRoles();
            return View(roles);
        }
        public IActionResult CreateRole()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRole(CreateRoleVM createRoleVM)
        {
            if (ModelState.IsValid)
            {
                string result = await _accountService.CreateRole(createRoleVM);
                if (result != null)
                {
                    ModelState.AddModelError(string.Empty, result.ToString());
                }
                else
                    return RedirectToAction(nameof(Index));
            }
            return View(createRoleVM);
        }
        public async Task<IActionResult> EditRole(string roleId)
        {
            var editRole = new EditRoleVM()
            {
                Role = _accountService.GetRoleById(roleId)
            };
            if (editRole.Role == null)
                return NotFound("Role Not Found!");
            editRole.UsersInRole = await _accountService.GetUsersInRole(editRole.Role);
            editRole.AllUsers =  _accountService.GetAllUsers();
            return View(editRole);
        }
        public async Task<IActionResult> AddToRole(string roleId,string userId)
        {
            var result =await _accountService.AddUserToRole(userId, roleId);
            return RedirectToAction(nameof(EditRole),new { roleId  = roleId });
        }
        public async Task<IActionResult> RemoveFromRole(string roleId, string userId)
        {
            var result = await _accountService.RemoveUserFromRole(userId, roleId);
            return RedirectToAction(nameof(EditRole), new { roleId = roleId });
        }

        public IActionResult DeleteRole(string roleId)
        {
            var role = _accountService.GetRoleById(roleId);
            if (role == null)
                return NotFound("Role Doesn't Exists!");

            return View(role);
        }

        [ActionName("DeleteRole")]
        [HttpPost]
        public async Task<IActionResult> DeleteRolePOST(string roleId)
        {
            var role = _accountService.GetRoleById(roleId);
            var result =await _accountService.DeleteRole(roleId);    
            if (result == null)
                return RedirectToAction(nameof(Index));
            ModelState.AddModelError(string.Empty, result);
            return View(role);
        }
    }
}
