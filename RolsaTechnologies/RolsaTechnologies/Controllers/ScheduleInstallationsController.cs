using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RolsaTechnologies.Data;
using RolsaTechnologies.Models;
using static System.Reflection.Metadata.BlobBuilder;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RolsaTechnologies.Controllers
{
    public class ScheduleInstallationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager; // Add this line

        public ScheduleInstallationsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager; // Initialize _userManager
        }

        [Authorize]

        // GET: ScheduleInstallations
        public async Task<IActionResult> Index()
        {
            // Retrieve the currently logged-in user's details
            var currentUser = await _userManager.GetUserAsync(User);
            var currentUserId = currentUser.Id;

            // Retrieve the roles of the currently logged-in user
            var userRole = await _userManager.GetRolesAsync(currentUser);

            List<ScheduleInstallation> userInstallations;

            // If the user is Admin or Professional, show all installations
            if (userRole.Contains("Admin") || userRole.Contains("Professional"))
            {
                userInstallations = await _context.ScheduleInstallation.ToListAsync();
            }
            else
            {
                // Regular users only see their own installations
                userInstallations = await _context.ScheduleInstallation
                                                   .Where(u => u.UserId == currentUserId)
                                                   .ToListAsync();
            }

            // Fetch all users whose IDs appear in the schedule installations
            var userIds = userInstallations.Select(u => u.UserId).Distinct();
            var users = await _context.Users
                                      .Where(u => userIds.Contains(u.Id))
                                      .ToDictionaryAsync(u => u.Id, u => u.Email);

            // Pass the dictionary of emails using ViewBag
            ViewBag.UserEmails = users;

            return View(userInstallations); // Return the list of schedule installations to the view
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
            // Ensure that we are passing the current date to generate time slots
            var availableSlots = GenerateAvailableTimeSlots(DateTime.Today);

            ViewBag.AvailableSlots = availableSlots;
            return View();
        }

        // POST: ScheduleInstallations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,ScheduledDate,ApplianceType,Address,Mobile,Notes")] ScheduleInstallation scheduleInstallation, string scheduledTime, string Postcode)
        {
            string UserName = User.Identity.Name;  // Get the currently logged-in user's username
            var currentUser = await _context.Users.FirstOrDefaultAsync(x => x.UserName == UserName);  // Retrieve the user details from the database based on the username

            // If the user is not found, return an unauthorized response
            if (currentUser == null)
            {
                return Unauthorized();
            }

            // Generate available time slots for the selected date
            var availableSlots = GenerateAvailableTimeSlots(scheduleInstallation.ScheduledDate);
            ViewBag.AvailableSlots = availableSlots;

            // Validate if the selected time is valid
            if (string.IsNullOrEmpty(scheduledTime) || !TimeSpan.TryParse(scheduledTime, out var time))
            {
                ModelState.AddModelError("ScheduledDate", "Invalid time selected.");
                return View(scheduleInstallation);
            }

            // Combine the selected date with the selected time
            scheduleInstallation.ScheduledDate = scheduleInstallation.ScheduledDate.Date + time;

            // Prevent selecting a past date/time
            if (scheduleInstallation.ScheduledDate < DateTime.Now)
            {
                ModelState.AddModelError("ScheduledDate", "You cannot select a past date or time.");
                return View(scheduleInstallation);
            }

            // Check if the selected date/time is already booked
            bool isSlotTaken = await _context.ScheduleInstallation.AnyAsync(s => s.ScheduledDate == scheduleInstallation.ScheduledDate);

            if (isSlotTaken)
            {
                ModelState.AddModelError("ScheduledDate", "This time slot is already booked. Please select another.");
                return View(scheduleInstallation);
            }

            // Custom validation for the address (ensure it’s not empty)
            if (string.IsNullOrEmpty(scheduleInstallation.Address))
            {
                ModelState.AddModelError("Address", "Street address is required.");
            }

            // Custom validation for UK postcode (simplified regex)
            var postcodeRegex = @"^([A-Z]{1,2}[0-9][A-Z0-9]?)\s?([0-9][A-Z]{2})$";
            if (string.IsNullOrEmpty(Postcode) || !Regex.IsMatch(Postcode, postcodeRegex))
            {
                ModelState.AddModelError("Postcode", "Invalid UK postcode.");
            }

            // Assign the logged-in user's ID to the consultation entry
            scheduleInstallation.UserId = currentUser.Id;
            scheduleInstallation.Address = $"{scheduleInstallation.Address}, {Postcode}";  // Concatenate Address and Postcode

            // Remove ModelState validation for UserId as it is assigned manually
            ModelState.Remove("UserId");

            if (ModelState.IsValid) // Check if the provided model data is valid
            {
                _context.Add(scheduleInstallation); // Add the consultation entry to the database
                await _context.SaveChangesAsync(); // Save changes asynchronously
                return RedirectToAction(nameof(Index)); // Redirect the user to the Index page after successful creation
            }

            return View(scheduleInstallation); // If the data is invalid, return the same view with validation errors
        }


        // Helper method to generate available time slots
        private List<TimeSpan> GenerateAvailableTimeSlots(DateTime scheduledDate)
        {
            List<TimeSpan> availableSlots = new List<TimeSpan>();

            // Generate time slots from 9 AM to 5 PM, every 15 minutes
            for (int hour = 9; hour < 17; hour++) // 9 AM to 5 PM
            {
                for (int minute = 0; minute < 60; minute += 15)
                {
                    availableSlots.Add(new TimeSpan(hour, minute, 0));
                }
            }

            // Retrieve the already booked time slots for the selected date (without time)
            var bookedSlots = _context.ScheduleInstallation.Where(s => s.ScheduledDate.Date == scheduledDate.Date).Select(s => s.ScheduledDate.TimeOfDay).ToList();

            // Remove booked time slots from the available slots list
            availableSlots = availableSlots.Where(slot => !bookedSlots.Contains(slot)).ToList();

            return availableSlots;
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
