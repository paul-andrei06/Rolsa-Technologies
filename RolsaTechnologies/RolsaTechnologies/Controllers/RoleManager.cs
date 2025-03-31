using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RolsaTechnologies.Models;  // Your application's models

public class RoleManagerController : Controller
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<IdentityUser> _userManager;

    // Inject RoleManager and UserManager
    public RoleManagerController(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
    {
        _roleManager = roleManager;
        _userManager = userManager;
    }

    // GET: RoleManager/Index
    public async Task<IActionResult> Index()
    {
        // Get all roles from the database
        var roles = await _roleManager.Roles.ToListAsync();
        return View(roles);  // Pass roles to the view
    }

    // GET: RoleManager/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: RoleManager/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(string roleName)
    {
        if (string.IsNullOrEmpty(roleName))
        {
            ModelState.AddModelError("", "Role name cannot be empty");
            return View();
        }

        // Create a new role
        var role = new IdentityRole(roleName);
        var result = await _roleManager.CreateAsync(role);

        if (result.Succeeded)
        {
            return RedirectToAction(nameof(Index));  // Redirect to the role list after creating the role
        }

        // If the role creation fails, add errors to the model state
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError("", error.Description);
        }

        return View();
    }

    // GET: RoleManager/Delete/roleId
    public async Task<IActionResult> Delete(string id)
    {
        var role = await _roleManager.FindByIdAsync(id);
        if (role == null)
        {
            return NotFound();
        }

        return View(role);
    }

    // POST: RoleManager/Delete/roleId
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        var role = await _roleManager.FindByIdAsync(id);
        if (role != null)
        {
            var result = await _roleManager.DeleteAsync(role);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index));  // Redirect to the role list after deletion
            }

            // If deletion fails, add errors to the model state
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }
        return NotFound();
    }

    // GET: RoleManager/AssignRoleToUser
    public IActionResult AssignRoleToUser()
    {
        return View();
    }

    // POST: RoleManager/AssignRoleToUser
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AssignRoleToUser(string userId, string roleName)
    {
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(roleName))
        {
            ModelState.AddModelError("", "User ID and Role name cannot be empty.");
            return View();
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            ModelState.AddModelError("", "User not found.");
            return View();
        }

        var result = await _userManager.AddToRoleAsync(user, roleName);
        if (result.Succeeded)
        {
            return RedirectToAction(nameof(Index));  // Redirect to the role list
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError("", error.Description);
        }

        return View();
    }


    

}