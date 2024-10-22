using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PetCare.Models;

namespace PetCare.Controllers
{
    public class PetsController : Controller
    {
        private readonly PetCareContext _context;

        public PetsController(PetCareContext context)
        {
            _context = context;
        }

        // GET: Pets
        public async Task<IActionResult> Index()
        {
            var pets = _context.Pets
                .Include(p => p.Customer)
                    .ThenInclude(c => c.Account)
                .Include(p => p.TypePet);
            return View(await pets.ToListAsync());
        }


        // GET: Pets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pet = await _context.Pets
                .Include(p => p.Customer)
                .Include(p => p.TypePet)
                .Include(p => p.InvoiceDetails)
                    .ThenInclude(id => id.Invoice)
                .FirstOrDefaultAsync(p => p.PetId == id);

            if (pet == null)
            {
                return NotFound();
            }

            return View(pet);
        }

        // GET: Pets/Create
        public IActionResult Create(int? customerId)
        {
            ViewBag.TypePetId = new SelectList(_context.TypePets, "TypePetId", "TypeName");

            if (customerId != null)
            {
                ViewBag.CustomerId = customerId;
            }
            else
            {
                ViewBag.CustomerId = new SelectList(_context.Customers.Include(c => c.Account), "CustomerId", "Account.FullName");
            }

            return View();
        }

        // POST: Pets/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Pet pet)
        {
            if (ModelState.IsValid)
            {
                _context.Pets.Add(pet);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Thêm thú cưng thành công!";
                return RedirectToAction("Details", "Customers", new { id = pet.CustomerId });
            }

            ViewBag.TypePetId = new SelectList(_context.TypePets, "TypePetId", "TypeName", pet.TypePetId);

            if (ViewBag.CustomerId == null)
            {
                ViewBag.CustomerId = new SelectList(_context.Customers.Include(c => c.Account), "CustomerId", "Account.FullName", pet.CustomerId);
            }

            return View(pet);
        }

        // GET: Pets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pet = await _context.Pets
                .Include(p => p.TypePet)
                .FirstOrDefaultAsync(p => p.PetId == id);

            if (pet == null)
            {
                return NotFound();
            }

            ViewBag.TypePetId = new SelectList(_context.TypePets, "TypePetId", "TypeName", pet.TypePetId);

            return View(pet);
        }


        // POST: Pets/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Pet pet)
        {
            if (id != pet.PetId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pet);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Cập nhật thú cưng thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PetExists(pet.PetId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", "Customers", new { id = pet.CustomerId });
            }

            ViewBag.TypePetId = new SelectList(_context.TypePets, "TypePetId", "TypeName", pet.TypePetId);

            return View(pet);
        }

        // GET: Pets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pet = await _context.Pets
                .Include(p => p.TypePet)
                .FirstOrDefaultAsync(p => p.PetId == id);

            if (pet == null)
            {
                return NotFound();
            }

            return View(pet);
        }


        // POST: Pets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pet = await _context.Pets.FindAsync(id);
            if (pet != null)
            {
                _context.Pets.Remove(pet);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Xoá thú cưng thành công!";
            }

            return RedirectToAction("Details", "Customers", new { id = pet.CustomerId });
        }

        private bool PetExists(int id)
        {
            return _context.Pets.Any(e => e.PetId == id);
        }
    }
}
