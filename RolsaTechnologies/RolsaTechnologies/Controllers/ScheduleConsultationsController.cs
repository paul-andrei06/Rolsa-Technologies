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
            return View(await _context.ScheduleConsultation.ToListAsync());
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
            if (ModelState.IsValid)
            {
                _context.Add(scheduleConsultation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(scheduleConsultation);
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
