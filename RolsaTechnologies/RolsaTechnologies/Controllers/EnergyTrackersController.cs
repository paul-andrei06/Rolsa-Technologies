using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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

        public EnergyTrackersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize] // This ensures that only logged in users are able to access this page

        // GET: EnergyTrackers
        public async Task<IActionResult> Index()
        {
            string user = User.Identity.Name; // Get the current logged-in user's name
            var currentUser = await _context.Users.FirstOrDefaultAsync(x => x.UserName == user); // Retrieve the current user's details from the database
            var currentUserId = currentUser.Id; // Get the current user's Id

            var userTracker = await _context.EnergyTracker.Where(t => t.UserId == currentUserId).ToListAsync(); // Gets all energy tracker records associated with the current user
            return View(userTracker); // Return the data to the view
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
