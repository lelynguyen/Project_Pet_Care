using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetCare.Models;

namespace PetCare.Controllers
{
    public class TypePetsController : Controller
    {
        private readonly PetCareContext _context;

        public TypePetsController(PetCareContext context)
        {
            _context = context;
        }

        // GET: TypePets
        public async Task<IActionResult> Index()
        {
            return View(await _context.TypePets.ToListAsync());
        }

        // GET: TypePets/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TypePets/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TypePetId,TypeName")] TypePet typePet)
        {
            if (ModelState.IsValid)
            {
                _context.Add(typePet);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Thêm loại thú cưng thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View(typePet);
        }

        // GET: TypePets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var typePet = await _context.TypePets.FindAsync(id);
            if (typePet == null)
            {
                return NotFound();
            }
            return View(typePet);
        }

        // POST: TypePets/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TypePetId,TypeName")] TypePet typePet)
        {
            if (id != typePet.TypePetId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(typePet);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Cập nhật loại thú cưng thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TypePetExists(typePet.TypePetId))
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
            return View(typePet);
        }

        // GET: TypePets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var typePet = await _context.TypePets
                .FirstOrDefaultAsync(m => m.TypePetId == id);
            if (typePet == null)
            {
                return NotFound();
            }

            return View(typePet);
        }

        // POST: TypePets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var typePet = await _context.TypePets.FindAsync(id);
            _context.TypePets.Remove(typePet);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Xoá loại thú cưng thành công!";
            return RedirectToAction(nameof(Index));
        }

        // GET: TypePets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var typePet = await _context.TypePets
                .FirstOrDefaultAsync(m => m.TypePetId == id);
            if (typePet == null)
            {
                return NotFound();
            }

            return View(typePet);
        }

        private bool TypePetExists(int id)
        {
            return _context.TypePets.Any(e => e.TypePetId == id);
        }
    }
}
