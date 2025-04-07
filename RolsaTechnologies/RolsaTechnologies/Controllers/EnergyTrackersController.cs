using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.DependencyResolver;
using RolsaTechnologies.Data;
using RolsaTechnologies.Models;

namespace RolsaTechnologies.Controllers
{
    public class EnergyTrackersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public EnergyTrackersController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
            _context = context;
        }

        [Authorize] // This ensures that only logged in users are able to access this page

        // GET: EnergyTrackers
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var currentUserId = currentUser.Id;

            var userRoles = await _userManager.GetRolesAsync(currentUser);

            List<EnergyTracker> trackers;

            if (userRoles.Contains("Admin") || userRoles.Contains("Professional"))
            {
                // Get all records
                trackers = await _context.EnergyTracker.ToListAsync();
            }
            else
            {
                // Only current user's records
                trackers = await _context.EnergyTracker
                                         .Where(t => t.UserId == currentUserId)
                                         .ToListAsync();
            }

            // Fetch all users whose IDs appear in the records
            var userIds = trackers.Select(t => t.UserId).Distinct();
            var users = await _context.Users
                                      .Where(u => userIds.Contains(u.Id))
                                      .ToDictionaryAsync(u => u.Id, u => u.Email);

            // Pass the dictionary using ViewBag
            ViewBag.UserEmails = users;

            return View(trackers);
        }

        // GET: EnergyTrackers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var energyTracker = await _context.EnergyTracker
                .FirstOrDefaultAsync(m => m.Id == id);
            if (energyTracker == null)
            {
                return NotFound();
            }

            return View(energyTracker);
        }

        // GET: EnergyTrackers/Create
        public IActionResult Create()
        {
            ViewBag.EnergyTypeList = new SelectList(new[] { "Electricity", "Gas" });
            return View();
        }

        // POST: EnergyTrackers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,Consumption,EnergyType,Date")] EnergyTracker energyTracker)
        {
            string UserName = User.Identity.Name; // Get the currently logged-in user's username
            var currentUser = await _context.Users.FirstOrDefaultAsync(x => x.UserName == UserName); // Retrieve the user details from the database based on the username

            // If the user is not found, return an unauthorized response
            if (currentUser == null)
            {
                return Unauthorized();
            }

            energyTracker.UserId = currentUser.Id; // Assign the logged-in user's ID to the entry
            energyTracker.Date = DateOnly.FromDateTime(DateTime.Now); // Set the date to the current date (DateOnly)

            ModelState.Remove("UserId"); // Remove ModelState validation for UserId as it is assigned manually

            if (ModelState.IsValid) // Check if the provided model data is valid
            {
                _context.Add(energyTracker); // Add the entry to the database
                await _context.SaveChangesAsync(); // Save changes asynchronously
                return RedirectToAction(nameof(Index)); // Redirect the user to the Index page after successful creation
            }

            ViewBag.EnergyTypeList = new SelectList(new[] { "Electricity", "Gas" });

            // If the data is invalid, return the same view with validation errors
            return View(energyTracker);
        }

        // GET: EnergyTrackers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var energyTracker = await _context.EnergyTracker.FindAsync(id);
            if (energyTracker == null)
            {
                return NotFound();
            }
            return View(energyTracker);
        }

        // POST: EnergyTrackers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,Consumption,EnergyType,Date")] EnergyTracker energyTracker)
        {
            if (id != energyTracker.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(energyTracker);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EnergyTrackerExists(energyTracker.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(energyTracker);
        }

        // GET: EnergyTrackers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var energyTracker = await _context.EnergyTracker
                .FirstOrDefaultAsync(m => m.Id == id);
            if (energyTracker == null)
            {
                return NotFound();
            }

            return View(energyTracker);
        }

        // POST: EnergyTrackers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var energyTracker = await _context.EnergyTracker.FindAsync(id);
            if (energyTracker != null)
            {
                _context.EnergyTracker.Remove(energyTracker);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EnergyTrackerExists(int id)
        {
            return _context.EnergyTracker.Any(e => e.Id == id);
        }
    }
}
