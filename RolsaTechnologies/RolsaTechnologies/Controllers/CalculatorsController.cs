using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<IdentityUser> _userManager;

        public CalculatorsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
            _context = context;
        }

        [Authorize] // This ensures that only logged in users are able to access this page

        // GET: Calculators
        public async Task<IActionResult> Index()
        {
            var userEmail = (await _userManager.FindByNameAsync(User.Identity.Name))?.Email;

            string user = User.Identity.Name; // Get the current logged-in user's name
            var currentUser = await _context.Users.FirstOrDefaultAsync(x => x.UserName == user); // Retrieve the current user's details from the database
            var currentUserId = currentUser.Id; // Get the current user's Id

            var usercalculations = await _context.Calculator.Where(u => u.UserId == currentUserId).ToListAsync(); // Gets all energy tracker records associated with the current user

            // Check if the user is an admin
            var isAdmin = await _userManager.IsInRoleAsync(currentUser, "Admin");

            // If the user is an admin, fetch all calculations, else fetch only the current user's calculations
            List<Calculator> calculators;
            if (isAdmin)
            {
                usercalculations = await _context.Calculator.ToListAsync(); // Admin can see all data
            }
            else
            {
                usercalculations = await _context.Calculator.Where(u => u.UserId == currentUserId).ToListAsync(); // Non-admin can only see their own data
            }


            ViewBag.UserEmail = userEmail;

            return View(usercalculations); // Return the data to the view
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
            string userName = User.Identity.Name;
            var currentUser = await _context.Users.FirstOrDefaultAsync(x => x.UserName == userName);

            if (currentUser == null)
            {
                return Unauthorized();
            }

            // validation for negative values
            var fieldsToValidate = new Dictionary<string, double>
            {
                { "ElectricityUsage", calculator.ElectricityUsage },
                { "GasUsage", calculator.GasUsage },
                { "CarMilesPerWeek", calculator.CarMilesPerWeek },
                { "CarFuelEfficiency", calculator.CarFuelEfficiency },
                { "PublicTransportMilesPerWeek", calculator.PublicTransportMilesPerWeek },
                { "WasteProducedPerWeek", calculator.WasteProducedPerWeek },
                { "MeatConsumptionPerWeek", calculator.MeatConsumptionPerWeek }
            };

            foreach (var field in fieldsToValidate)
            {
                if (field.Value < 0)
                {
                    ModelState.AddModelError(field.Key, $"{field.Key} cannot be negative.");
                }
            }

            calculator.UserId = currentUser.Id;
            calculator.CalculateFootprint();

            ModelState.Remove("UserId");

            if (ModelState.IsValid)
            {
                _context.Add(calculator);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

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
