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
    public class CalculatorsController : Controller
    {
        private readonly ApplicationDbContext _context;
        
        public CalculatorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize] // This ensures that only logged in users are able to access this page

        // GET: Calculators
        public async Task<IActionResult> Index()
        {
            return View(await _context.Calculator.ToListAsync());
        }

        // GET: Calculators/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var calculator = await _context.Calculator
                .FirstOrDefaultAsync(m => m.Id == id);
            if (calculator == null)
            {
                return NotFound();
            }

            return View(calculator);
        }

        // GET: Calculators/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Calculators/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,ElectricityUsage,GasUsage,CarMilesPerWeek,CarFuelEfficiency,PublicTransportMilesPerWeek,WasteProducedPerWeek,RecyclingHabits,MeatConsumptionPerWeek,CalculatedCarbonFootprint,DateCalculated")] Calculator calculator)
        {
            string userName = User.Identity.Name;  // Get the currently logged-in user's username
            var currentUser = await _context.Users.FirstOrDefaultAsync(x => x.UserName == userName); // Retrieve the user details from the database based on the username

            // If the user is not found, return an unauthorized response
            if (currentUser == null)
            {
                return Unauthorized();
            }
           
            calculator.UserId = currentUser.Id; // Assign the logged-in user's ID to the calculator entry
            calculator.CalculateFootprint(); // Call the CalculateFootprint method to compute the carbon footprint

            ModelState.Remove("UserId"); // Remove ModelState validation for UserId as it is assigned manually

            if (ModelState.IsValid) // Check if the provided model data is valid
            {
                _context.Add(calculator); // Add the calculator entry to the database
                await _context.SaveChangesAsync(); // Save changes asynchronously
                return RedirectToAction(nameof(Index)); // Redirect the user to the Index page after successful creation
            }

            // If the data is invalid, return the same view with validation errors
            return View(calculator);

        }

        // GET: Calculators/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var calculator = await _context.Calculator.FindAsync(id);
            if (calculator == null)
            {
                return NotFound();
            }
            return View(calculator);
        }

        // POST: Calculators/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,ElectricityUsage,GasUsage,CarMilesPerWeek,CarFuelEfficiency,PublicTransportMilesPerWeek,WasteProducedPerWeek,RecyclingHabits,MeatConsumptionPerWeek,CalculatedCarbonFootprint,DateCalculated")] Calculator calculator)
        {
            if (id != calculator.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(calculator);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CalculatorExists(calculator.Id))
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
            return View(calculator);
        }

        // GET: Calculators/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var calculator = await _context.Calculator
                .FirstOrDefaultAsync(m => m.Id == id);
            if (calculator == null)
            {
                return NotFound();
            }

            return View(calculator);
        }

        // POST: Calculators/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var calculator = await _context.Calculator.FindAsync(id);
            if (calculator != null)
            {
                _context.Calculator.Remove(calculator);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CalculatorExists(int id)
        {
            return _context.Calculator.Any(e => e.Id == id);
        }
    }
}
