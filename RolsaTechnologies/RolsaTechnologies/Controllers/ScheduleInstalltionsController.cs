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
    public class ScheduleInstalltionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ScheduleInstalltionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize]

        // GET: ScheduleInstalltions
        public async Task<IActionResult> Index()
        {
            string user = User.Identity.Name; // Gets the currently logged in users username
            var currentUser = await _context.Users.FirstOrDefaultAsync(x => x.UserName == user); // Gets the details of that user from the database
            var currentUserId = currentUser.Id; // Gets the users Id
            var usercalculations = await _context.Calculator.Where(u => u.UserId == currentUserId).ToListAsync(); // Gets all of the calculation forms associated with that particular user

            return View(usercalculations); //Displays only the currently logged in users calculations
        }

        // GET: ScheduleInstalltions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var scheduleInstalltion = await _context.ScheduleInstalltion
                .FirstOrDefaultAsync(m => m.Id == id);
            if (scheduleInstalltion == null)
            {
                return NotFound();
            }

            return View(scheduleInstalltion);
        }

        // GET: ScheduleInstalltions/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ScheduleInstalltions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,ScheduledDate,ApplianceType,Address,Mobile,Notes")] ScheduleInstalltion scheduleInstalltion)
        {
            if (ModelState.IsValid)
            {
                _context.Add(scheduleInstalltion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(scheduleInstalltion);
        }

        // GET: ScheduleInstalltions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var scheduleInstalltion = await _context.ScheduleInstalltion.FindAsync(id);
            if (scheduleInstalltion == null)
            {
                return NotFound();
            }
            return View(scheduleInstalltion);
        }

        // POST: ScheduleInstalltions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,ScheduledDate,ApplianceType,Address,Mobile,Notes")] ScheduleInstalltion scheduleInstalltion)
        {
            if (id != scheduleInstalltion.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(scheduleInstalltion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ScheduleInstalltionExists(scheduleInstalltion.Id))
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
            return View(scheduleInstalltion);
        }

        // GET: ScheduleInstalltions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var scheduleInstalltion = await _context.ScheduleInstalltion
                .FirstOrDefaultAsync(m => m.Id == id);
            if (scheduleInstalltion == null)
            {
                return NotFound();
            }

            return View(scheduleInstalltion);
        }

        // POST: ScheduleInstalltions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var scheduleInstalltion = await _context.ScheduleInstalltion.FindAsync(id);
            if (scheduleInstalltion != null)
            {
                _context.ScheduleInstalltion.Remove(scheduleInstalltion);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ScheduleInstalltionExists(int id)
        {
            return _context.ScheduleInstalltion.Any(e => e.Id == id);
        }
    }
}
