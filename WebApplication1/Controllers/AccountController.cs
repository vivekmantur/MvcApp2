using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.ViewModel;

namespace WebApplication1.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<UserRegistration> userManager;
        private readonly SignInManager<UserRegistration> signInManager;
        public IEmailSender emailSender;
        public AccountController(UserManager<UserRegistration> userManager,
            SignInManager<UserRegistration> signInManager,IEmailSender emailSender)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.emailSender = emailSender;
        }
        [HttpGet, AllowAnonymous]
        public IActionResult Register()
        {
            UserViewModel model = new UserViewModel();
            return View(model);
        }

        [HttpPost, AllowAnonymous]
        public IActionResult Register(UserViewModel request)
        {
            if (ModelState.IsValid)
            {

                var user = new UserRegistration
                {
                    UserName = request.UserName,
                    Email = request.Email,
                    PhoneNumber = request.PhoneNumber,
                    Address = request.Address,
                };
                var result = userManager.CreateAsync(user, request.Password).Result;
                if (result.Succeeded)
                {
                    return RedirectToAction("Login");
                }
            }
            return View(request);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(String? ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            UserLogin model = new UserLogin();
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Login(UserLogin model, String? ReturnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = userManager.FindByEmailAsync(model.Email).Result;
                if (user == null)
                {
                    ModelState.AddModelError("message", "Invalid credentials");
                    return View(model);
                }

                var passwordCheck = userManager.CheckPasswordAsync(user, model.Password).Result;
                if (!passwordCheck)
                {
                    ModelState.AddModelError("message", "Invalid credentials");
                    return View(model);
                }

                var result = signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, true).Result;
                if (result.Succeeded)
                {
                    if (ReturnUrl == null)
                    {
                        return RedirectToAction("Dashboard");
                    }
                    else
                    {
                        return Redirect(ReturnUrl);
                    }
                }
                else
                {
                    ModelState.AddModelError("message", "Invalid login attempt");
                    return View(model);
                }
            }
            return View(model);
        }

        [Authorize]
        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult Logout()
        {
            signInManager.SignOutAsync().Wait();
            return RedirectToAction("login", "account");
        }
        // ViewProfile GET Action (Synchronous version)
        [HttpGet]
        public IActionResult ViewProfile(string email)
        {
            // Get the user by email synchronously
            var user = userManager.FindByEmailAsync(email).Result;

            if (user == null)
            {
                return NotFound();
            }

            // Map the user's data to the ViewModel
            var userModel = new UserViewModel
            {
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
            };

            return View(userModel);
        }

        // EditProfile GET Action (Synchronous version)
        [HttpGet]
        public IActionResult EditProfile()
        {
            // Get the current logged-in user synchronously
            var user = userManager.GetUserAsync(User).Result;

            if (user == null)
            {
                return NotFound();
            }

            // Map the user's data to the ViewModel
            var userViewModel = new UserViewModel
            {
                UserName = user.UserName,
                Email = user.Email,
                Address = user.Address,
                PhoneNumber = user.PhoneNumber
            };

            return View(userViewModel);
        }

        // EditProfile POST Action (Synchronous version)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditProfile(ProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Get the current logged-in user synchronously
                var user = userManager.GetUserAsync(User).Result;

                if (user == null)
                {
                    return NotFound();
                }

                // Update the user's profile information
                user.UserName = model.UserName;
                user.Email = model.Email;
                user.Address = model.Address;
                user.PhoneNumber = model.PhoneNumber;

                // Save the changes to the database synchronously
                var result = userManager.UpdateAsync(user).Result;

                if (result.Succeeded)
                {
                    return RedirectToAction("ViewProfile", new { email = user.Email });
                }
                else
                {
                    // If there are any errors, display them
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            // If there were validation errors, redisplay the form with the model
            return View(model);
        }

        [HttpGet]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Get the current logged-in user
                var user = userManager.GetUserAsync(User).Result;

                if (user == null)
                {
                    return NotFound();
                }

                // Check if the current password is correct
                var passwordCheckResult = userManager.CheckPasswordAsync(user, model.CurrentPassword).Result;
                if (!passwordCheckResult)
                {
                    ModelState.AddModelError(string.Empty, "The current password is incorrect.");
                    return View(model);
                }

                // Check if the new password and confirmation password match
                if (model.NewPassword != model.ConfirmPassword)
                {
                    ModelState.AddModelError(string.Empty, "The new password and confirmation password do not match.");
                    return View(model);
                }

                // Change the password
                var result = userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword).Result; // Change password synchronously

                if (result.Succeeded)
                {
                    return RedirectToAction("ViewProfile", new { email = user.Email });
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user == null || !(await userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToAction("ForgotPasswordConfirmation");
                }

                // Generate password reset token
                var token = await userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { token = token, email = model.Email }, protocol: HttpContext.Request.Scheme);

                await emailSender.SendEmailAsync(model.Email, "Reset Your Password",
                    $"Please reset your password by clicking here: <a href='{callbackUrl}'>Reset Password</a>");

                return RedirectToAction("ForgotPasswordConfirmation");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult ResetPassword(string token = null, string email = null)
        {
            if (token == null || email == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var model = new ResetPasswordViewModel { Token = token, Email = email };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    return RedirectToAction("ResetPasswordConfirmation");
                }

                var result = await userManager.ResetPasswordAsync(user, model.Token, model.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction("ResetPasswordConfirmation");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }
    }
}
