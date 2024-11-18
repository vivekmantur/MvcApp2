using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
using WebApplication1.ViewModel;

namespace WebApplication1.Controllers
{
    public class RoleController : Controller
    {
        private readonly UserManager<UserRegistration> _userManager;
        private readonly SignInManager<UserRegistration> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleController(UserManager<UserRegistration> userManager,
                                 SignInManager<UserRegistration> signInManager,
                                 RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        // Action method to change the user's role
        [HttpGet]
        public async Task<IActionResult> ChangeRole(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("User ID is required.");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Get current roles of the user
            var roles = await _userManager.GetRolesAsync(user);

            // View model to pass the user info and current roles
            var model = new ChangeRoleViewModel
            {
                UserId = user.Id,
                UserName = user.UserName,
                CurrentRole = roles.FirstOrDefault() // Assuming user has only one role
            };

            // Get all available roles
            model.AvailableRoles = await _roleManager.Roles
                .Where(r => r.Name != model.CurrentRole) // Don't allow user to select their current role
                .Select(r => r.Name).ToListAsync();

            return View(model);
        }

        // Action method to handle role change (POST)
        [HttpPost]
        public async Task<IActionResult> ChangeRole(ChangeRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.UserId);
                if (user == null)
                {
                    return NotFound("User not found.");
                }

                // Get the user's current roles
                var currentRoles = await _userManager.GetRolesAsync(user);

                // Remove the user from all roles
                var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                if (!removeResult.Succeeded)
                {
                    ModelState.AddModelError(string.Empty, "Failed to remove current roles.");
                    return View(model);
                }

                // Add the new role
                var addResult = await _userManager.AddToRoleAsync(user, model.NewRole);
                if (!addResult.Succeeded)
                {
                    ModelState.AddModelError(string.Empty, "Failed to assign the new role.");
                    return View(model);
                }

                // Optionally sign the user in (you can choose not to)
                await _signInManager.RefreshSignInAsync(user);

                // Redirect to some confirmation page
                return RedirectToAction("Index", "Home");
            }

            // If the model state is invalid, return the view with errors
            return View(model);
        }
    }
}
