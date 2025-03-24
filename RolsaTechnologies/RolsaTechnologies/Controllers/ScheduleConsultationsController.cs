using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RolsaTechnologies.Data;
using RolsaTechnologies.Models;

namespace RolsaTechnologies.Controllers
{
    public class ScheduleConsultationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ScheduleConsultationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize]

        // GET: ScheduleConsultations
        public async Task<IActionResult> Index()
        {
            string user = User.Identity.Name; // Get the current logged -in user's name
            var currentUser = _context.Users.FirstOrDefault(x => x.UserName == user); // Retrieve the current user's details from the database
            var currentUserId = currentUser.Id; // Gets the current user's Id

            var userConsultation = await _context.ScheduleConsultation.Where(c => c.UserId == currentUserId).ToListAsync(); // Gets all energy tracker records associated with the current user

            return View(userConsultation); // Return the data to the view
        }

        // GET: ScheduleConsultations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var scheduleConsultation = await _context.ScheduleConsultation
                .FirstOrDefaultAsync(m => m.Id == id);
            if (scheduleConsultation == null)
            {
                return NotFound();
            }

            return View(scheduleConsultation);
        }

        // GET: ScheduleConsultations/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ScheduleConsultations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,ScheduledDate,ContactMethod,Mobile,ContactEmail,Notes")] ScheduleConsultation scheduleConsultation)
        {
            string UserName = User.Identity.Name;  // Get the currently logged-in user's username
            var currentUser = await _context.Users.FirstOrDefaultAsync(x => x.UserName == UserName);  // Retrieve the user details from the database based on the username

            // If the user is not found, return an unauthorized response
            if (currentUser == null)
            {
                return Unauthorized();
            }

            scheduleConsultation.UserId = currentUser.Id; // Assign the logged-in user's ID to the calculator entry

            ModelState.Remove("UserId"); // Remove ModelState validation for UserId as it is assigned manually

            if (ModelState.IsValid) // Check if the provided model data is valid
            {
                _context.Add(scheduleConsultation); // Add the consultation entry to the database
                await _context.SaveChangesAsync(); // Save changes asynchronously
                return RedirectToAction(nameof(Index)); // Redirect the user to the Index page after successful creation
            }
            return View(scheduleConsultation); // If the data is invalid, return the same view with validation errors
        }

        // GET: ScheduleConsultations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var scheduleConsultation = await _context.ScheduleConsultation.FindAsync(id);
            if (scheduleConsultation == null)
            {
                return NotFound();
            }
            return View(scheduleConsultation);
        }

        // POST: ScheduleConsultations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,ScheduledDate,ContactMethod,Mobile,ContactEmail,Notes")] ScheduleConsultation scheduleConsultation)
        {
            if (id != scheduleConsultation.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(scheduleConsultation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ScheduleConsultationExists(scheduleConsultation.Id))
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
            return View(scheduleConsultation);
        }

        // GET: ScheduleConsultations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var scheduleConsultation = await _context.ScheduleConsultation
                .FirstOrDefaultAsync(m => m.Id == id);
            if (scheduleConsultation == null)
            {
                return NotFound();
            }

            return View(scheduleConsultation);
        }

        // POST: ScheduleConsultations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var scheduleConsultation = await _context.ScheduleConsultation.FindAsync(id);
            if (scheduleConsultation != null)
            {
                _context.ScheduleConsultation.Remove(scheduleConsultation);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ScheduleConsultationExists(int id)
        {
            return _context.ScheduleConsultation.Any(e => e.Id == id);
        }
    }
}
