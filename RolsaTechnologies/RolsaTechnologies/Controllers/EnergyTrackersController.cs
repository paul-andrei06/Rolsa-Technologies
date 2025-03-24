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
    public class EnergyTrackersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EnergyTrackersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize]

        // GET: EnergyTrackers
        public async Task<IActionResult> Index()
        {
            return View(await _context.EnergyTracker.ToListAsync());
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
            if (ModelState.IsValid)
            {
                _context.Add(energyTracker);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
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
