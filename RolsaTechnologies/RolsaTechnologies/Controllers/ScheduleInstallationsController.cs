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
    public class ScheduleInstallationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ScheduleInstallationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize]

        // GET: ScheduleInstallations
        public async Task<IActionResult> Index()
        {
            string user = User.Identity.Name; // Gets the currently logged in users username
            var currentUser = await _context.Users.FirstOrDefaultAsync(x => x.UserName == user); // Gets the details of that user from the database
            var currentUserId = currentUser.Id; // Gets the users Id
            var userInstallations = await _context.ScheduleInstallation.Where(u => u.UserId == currentUserId).ToListAsync(); // Gets all of the forms associated with that particular user

            return View(userInstallations); //Displays only the currently logged in users
        }

        // GET: ScheduleInstallations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var scheduleInstalltion = await _context.ScheduleInstallation.FirstOrDefaultAsync(m => m.Id == id);
            if (scheduleInstalltion == null)
            {
                return NotFound();
            }

            return View(scheduleInstalltion);
        }

        // GET: ScheduleInstallations/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ScheduleInstallations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,ScheduledDate,ApplianceType,Address,Mobile,Notes")] ScheduleInstallation scheduleInstallation)
        {
            string UserName = User.Identity.Name; // Get the currently logged-in user's username
            var currentUser = await _context.Users.FirstOrDefaultAsync(x => x.UserName == UserName);  // Retrieve the user details from the database based on the username

            // If the user is not found, return an unauthorized response
            if (currentUser == null)
            {
                return Unauthorized();
            }

            scheduleInstallation.UserId = currentUser.Id; // Assign the logged-in user's ID to the entry

            ModelState.Remove("UserId"); // Remove ModelState validation for UserId as it is assigned manually

            if (ModelState.IsValid)
            {
                _context.Add(scheduleInstallation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(scheduleInstallation);
        }

        // GET: ScheduleInstallations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var scheduleInstalltion = await _context.ScheduleInstallation.FindAsync(id);
            if (scheduleInstalltion == null)
            {
                return NotFound();
            }
            return View(scheduleInstalltion);
        }

        // POST: ScheduleInstallations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,ScheduledDate,ApplianceType,Address,Mobile,Notes")] ScheduleInstallation scheduleInstallation)
        {
            if (id != scheduleInstallation.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(scheduleInstallation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ScheduleInstalltionExists(scheduleInstallation.Id))
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
            return View(scheduleInstallation);
        }

        // GET: ScheduleInstallations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var scheduleInstalltion = await _context.ScheduleInstallation.FirstOrDefaultAsync(m => m.Id == id);
            if (scheduleInstalltion == null)
            {
                return NotFound();
            }

            return View(scheduleInstalltion);
        }

        // POST: ScheduleInstallations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var scheduleInstalltion = await _context.ScheduleInstallation.FindAsync(id);
            if (scheduleInstalltion != null)
            {
                _context.ScheduleInstallation.Remove(scheduleInstalltion);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ScheduleInstalltionExists(int id)
        {
            return _context.ScheduleInstallation.Any(e => e.Id == id);
        }
    }
}
