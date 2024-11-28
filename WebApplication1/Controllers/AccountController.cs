using System.Collections.Concurrent;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.ViewModel;

namespace WebApplication1.Controllers
{
   
    public class AccountController : Controller
    {
        private readonly UserManager<UserRegistration> userManager;
        private readonly SignInManager<UserRegistration> signInManager;
        public EmailService emailService;
        public WebApplication1Context context;
        private static readonly ConcurrentDictionary<string, (string Otp, DateTime Expiration)> _otps = new();
        public AccountController(UserManager<UserRegistration> userManager,
            SignInManager<UserRegistration> signInManager,EmailService emailService,WebApplication1Context context)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.emailService = emailService;
            this.context = context;
        }
        [HttpGet, AllowAnonymous]
        public IActionResult Register()
        {
            UserViewModel model = new UserViewModel();
            return View(model);
        }

        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> Register(UserViewModel request)
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

                var result = await userManager.CreateAsync(user, request.Password);
                var res = await userManager.AddToRoleAsync(user, "User");
                if (result.Succeeded&&res.Succeeded)
                {
                    return RedirectToAction("Login");
                }
            }
            return View(request);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(String? ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            UserLogin model = new UserLogin
            { 
                ReturnUrl = ReturnUrl,
                ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync())
            };
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(UserLogin model, string? ReturnUrl)
        {
            if (ModelState.IsValid)
            {
                UserRegistration user = await userManager.FindByEmailAsync(model.Email);

                if (user == null)
                {
                    ModelState.AddModelError("message", "Invalid credentials");
                    return View(model);
                }

                var passwordCheck = await userManager.CheckPasswordAsync(user, model.Password);
                
                if (!passwordCheck)
                {
                    ModelState.AddModelError("message", "Invalid credentials");
                    return View(model);
                }

                var result = await signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, true);
                if (result.Succeeded)
                {
                    if (ReturnUrl == null)
                    {
                        return RedirectToAction("Dashboard","User");
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
        
        /// <summary>
        /// action to login user with google
        /// </summary>
        /// <param name="provider">external provider </param>
        /// <param name="returnUrl">return url input string</param>
        /// <returns>External Login View</returns>
        [AllowAnonymous]
        [HttpPost]
        public IActionResult ExternalLogin(string provider, string returnUrl)
        {
            var redirectUrl = Url.Action(action: "ExternalLoginCallback", controller: "Account", values: new { ReturnUrl = returnUrl });

            // Configure external authentication properties
            var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            //to get the account selection screen
            properties.Items["prompt"] = "select_account";

            // This will redirect the user to the external provider's login page
            return new ChallengeResult(provider, properties);
        }
        /// <summary>
        /// Login functionality by verifying user email
        /// </summary>
        /// <param name="returnUrl">return url</param>
        /// <param name="remoteError">authenti</param>
        /// <returns></returns>
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string? returnUrl, string? remoteError)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            UserLogin loginViewModel = new UserLogin
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync())?.ToList() ?? new List<AuthenticationScheme>()
            };
            if(remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
                return View("Login", loginViewModel);
            }

            // Get the login information about the user from the external login provider
            var info = await signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ModelState.AddModelError(string.Empty, "Error loading external login information.");
                return View("Login", loginViewModel);
            }

            // If the user already has a login (i.e., if there is a record in AspNetUserLogins table)
            var signInResult = await signInManager.ExternalLoginSignInAsync(info.LoginProvider,
                info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

            if (signInResult.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                //If there is no user record in AspNetUsers Table then he should redirect to Register Table
                // Get the email claim value from the external login info
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                if (email != null)
                {
                    // The user is not found, show an error and redirect to registration.
                    return RedirectToAction("Register", "Account");
                }
                return View("Error");
            }
        }

        public IActionResult Logout()
        {
            signInManager.SignOutAsync().Wait();
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }
        public IActionResult PrimeCarDealsLandingPage()
        {
            var unsoldCarList = context.CarDetails.ToList();
           unsoldCarList = unsoldCarList.Where(i => i.Status == "sold").ToList();
            return View(unsoldCarList);
        }
        

        [HttpGet]
        public IActionResult ViewProfile(string email)
        {
            // Get the user by email synchronously
            var user = userManager.FindByEmailAsync(email).Result;

            if (user == null)
            {
                return NotFound();
            }

            //getting any payments for cars buyed for user
            var payments=context.Payments.ToList();
            payments = payments.Where(i => i.BuyyerId == user.Id).ToList();

            //getting details of cars if user has sold recently
            var carsSold=context.CarsSold.ToList();
            carsSold=carsSold.Where(i=>i.UserId == user.Id).ToList();

            // Map the user's data to the ViewModel
            var userModel = new UserViewModel
            {
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
            };

            ViewBag.carbuys=payments;

            ViewBag.carsold=carsSold;

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
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
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
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model, string otp = null)
        {
            // If no TempData Email exists, we are in the first step (sending OTP)
            if (TempData["Email"] == null)
            {
                if (ModelState.IsValid)
                {
                    var user = await userManager.FindByEmailAsync(model.Email);
                    if (user == null)
                    {
                        ModelState.AddModelError("", "Email not found.");
                        return View();
                    }

                    // Generate OTP
                    string generatedOtp = new Random().Next(100000, 999999).ToString();
                    DateTime expiration = DateTime.UtcNow.AddMinutes(3);

                    // Store OTP with expiration
                    _otps[model.Email] = (generatedOtp, expiration);

                    // Send OTP via email
                    await emailService.SendEmailAsync(model.Email, "Password Reset OTP", $"Your OTP is {generatedOtp}. It will expire in 3 minutes.");

                    TempData["Email"] = model.Email;
                    TempData.Keep("Email");
                    return View();
                }
            }
            else
            {
                // Email already provided, validate OTP
                string email = TempData["Email"].ToString();
                TempData.Keep("Email");

                if (string.IsNullOrEmpty(otp))
                {
                    ModelState.AddModelError("", "Please enter the OTP.");
                    return View();
                }

                // Check if OTP is valid and not expired
                if (_otps.TryGetValue(email, out var otpDetails))
                {
                    if (otpDetails.Otp == otp && otpDetails.Expiration > DateTime.UtcNow)
                    {
                        // OTP verified, remove from store
                        _otps.TryRemove(email, out _);

                        // Redirect to reset password
                        TempData["Email"] = email;
                        return RedirectToAction("ResetPassword");
                    }
                }

                ModelState.AddModelError("", "Invalid or expired OTP.");
            }

            return View();
        }

        [HttpGet]
        public IActionResult ResetPassword()
        {
            if (TempData["Email"] == null)
                return RedirectToAction("ForgotPassword");

            TempData.Keep("Email");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (TempData["Email"] == null)
                return RedirectToAction("ForgotPassword");

            var email = TempData["Email"].ToString();

            if (ModelState.IsValid && model.Password == model.ConfirmPassword)
            {
                var user = await userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    ModelState.AddModelError("", "Invalid email.");
                    return View(model);
                }

                // Reset the password
                var resetResult = await userManager.RemovePasswordAsync(user);
                if (resetResult.Succeeded)
                {
                    await userManager.AddPasswordAsync(user, model.Password);
                    return RedirectToAction("Login");
                }

                foreach (var error in resetResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }

      
    }
}
