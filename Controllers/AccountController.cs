using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Task9Identity.Models.VMs;
using Task9Identity.Services.IServices;

namespace Task9Identity.Controllers
{
    public class AccountController : Controller
    {
        IAccountService _accountService;
        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public IActionResult Login()
        {
            return View();
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> LoginAsync(LoginVM loginVM)
        {
            if(ModelState.IsValid)
            {
                var result = await _accountService.Login(loginVM);
                if (result)
                    return RedirectToAction("Index", "Home");
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid Login Attempt!");
                    TempData["error"] = "Login Failed";
                }
                    
            }
            return View(loginVM);
        }
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (ModelState.IsValid)
            {
                string result = await _accountService.RegisterNewUser(registerVM);
                if (result != null)
                {
                    ModelState.AddModelError(string.Empty, result.ToString());
                }
                else
                    return RedirectToAction("Index", "Home");
            }
            return View(registerVM);
        }

        public async Task<IActionResult> Logout()
        {
            await _accountService.Logout();
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public IActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> ResetPassword(ResetPasswrodVM resetPasswrodVM)
        {
            if (ModelState.IsValid)
            {
                string result = await _accountService.ResetPassword(HttpContext.User.Identity.Name, resetPasswrodVM);
                if (result != null)
                {
                    ModelState.AddModelError(string.Empty, result.ToString());
                }
                else
                    return RedirectToAction(nameof(Login));
            }
            return View(resetPasswrodVM);
        }
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        [ActionName("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword(ForgetPasswordVM vm)
        {
            if(ModelState.IsValid)
            {
                var token =await _accountService.GeneratePasswordResetToken(vm.Email);
                if (token == null)
                    return NotFound("Account Not Found");
                Request.QueryString.Add("email",vm.Email);
                Request.QueryString.Add("token", token);
                var forgetPasswordReset = new ForgetPasswordSetNewVM()
                {
                    Email = vm.Email,
                    Token = token
                };
                return View(nameof(ForgotPasswordReset), forgetPasswordReset);
                //return View(nameof(ForgotPasswordReset));
            }
            return View();
        }

        public IActionResult ForgotPasswordReset(string email,string token)
        {
            if(token == null || email ==null)
                return NotFound();
            var forgetPasswordReset = new ForgetPasswordSetNewVM()
            {
                Email = email,
                Token = token
            };
            return View(forgetPasswordReset);
        }
        [HttpPost]
        public async Task<IActionResult> ForgotPasswordReset(ForgetPasswordSetNewVM forgetPasswordSetNewVM)
        {

            var result =await _accountService.ForgotRestPassword(forgetPasswordSetNewVM);
            if (result==null)
                return RedirectToAction(nameof(Login));
            else 
                return Problem(result);
        }

    }
}
