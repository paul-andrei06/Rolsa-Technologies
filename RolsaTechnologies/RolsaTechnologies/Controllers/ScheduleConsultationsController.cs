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

        // GET: ScheduleConsultation/Create
        public IActionResult Create()
        {
            // Ensure that we are passing the current date to generate time slots
            var availableSlots = GenerateAvailableTimeSlots(DateTime.Today);

            ViewBag.AvailableSlots = availableSlots;

            return View();
        }

        // POST: ScheduleConsultations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ScheduleConsultation scheduleConsultation, string scheduledTime)
        {
            string UserName = User.Identity.Name;  // Get the currently logged-in user's username
            var currentUser = await _context.Users.FirstOrDefaultAsync(x => x.UserName == UserName);  // Retrieve the user details from the database based on the username

            // If the user is not found, return an unauthorized response
            if (currentUser == null)
            {
                return Unauthorized();
            }

            // Generate available time slots for the selected date
            var availableSlots = GenerateAvailableTimeSlots(scheduleConsultation.ScheduledDate);
            ViewBag.AvailableSlots = availableSlots;

            // Validate if the selected time is valid
            if (string.IsNullOrEmpty(scheduledTime) || !TimeSpan.TryParse(scheduledTime, out var time))
            {
                ModelState.AddModelError("ScheduledDate", "Invalid time selected.");
                return View(scheduleConsultation);
            }

            // Combine the selected date with the selected time
            scheduleConsultation.ScheduledDate = scheduleConsultation.ScheduledDate.Date + time;

            // Prevent selecting a past date/time
            if (scheduleConsultation.ScheduledDate < DateTime.Now)
            {
                ModelState.AddModelError("ScheduledDate", "You cannot select a past date or time.");
                return View(scheduleConsultation);
            }

            // Check if the selected date/time is already booked
            bool isSlotTaken = await _context.ScheduleConsultation.AnyAsync(s => s.ScheduledDate == scheduleConsultation.ScheduledDate);

            if (isSlotTaken)
            {
                ModelState.AddModelError("ScheduledDate", "This time slot is already booked. Please select another.");
                return View(scheduleConsultation);
            }

            // Assign the logged-in user's ID to the consultation entry
            scheduleConsultation.UserId = currentUser.Id;

            // Remove ModelState validation for UserId as it is assigned manually
            ModelState.Remove("UserId");

            if (ModelState.IsValid) // Check if the provided model data is valid
            {
                _context.Add(scheduleConsultation); // Add the consultation entry to the database
                await _context.SaveChangesAsync(); // Save changes asynchronously
                return RedirectToAction(nameof(Index)); // Redirect the user to the Index page after successful creation
            }

            return View(scheduleConsultation); // If the data is invalid, return the same view with validation errors
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
            var bookedSlots = _context.ScheduleConsultation.Where(s => s.ScheduledDate.Date == scheduledDate.Date).Select(s => s.ScheduledDate.TimeOfDay).ToList();

            // Remove booked time slots from the available slots list
            availableSlots = availableSlots.Where(slot => !bookedSlots.Contains(slot)).ToList();

            return availableSlots;
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
