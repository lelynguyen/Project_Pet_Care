using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PetCare.Models;

namespace PetCare.Controllers
{
    public class CustomersController : Controller
    {
        private readonly PetCareContext _context;

        public CustomersController(PetCareContext context)
        {
            _context = context;
        }

        // GET: Customers
        public async Task<IActionResult> Index()
        {
            var customers = _context.Customers
                .Include(c => c.Account);
            return View(await customers.ToListAsync());
        }

        // GET: Customers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .Include(c => c.Account)
                .Include(c => c.Pets)
                .ThenInclude(p => p.TypePet)
                .FirstOrDefaultAsync(c => c.CustomerId == id);

            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // GET: Customers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Customers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Customer customer)
        {
            if (ModelState.IsValid)
            {
                // Thêm Account
                _context.Accounts.Add(customer.Account);
                await _context.SaveChangesAsync();

                // Gán AccountId cho Customer
                customer.AccountId = customer.Account.AccountId;

                // Thêm Customer
                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Thêm khách hàng thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        // GET: Customers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .Include(c => c.Account)
                .FirstOrDefaultAsync(c => c.CustomerId == id);

            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Customers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Customer customer)
        {
            if (id != customer.CustomerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Cập nhật Account
                    var existingAccount = await _context.Accounts.FindAsync(customer.Account.AccountId);
                    if (existingAccount == null)
                    {
                        return NotFound();
                    }

                    existingAccount.FullName = customer.Account.FullName;
                    existingAccount.PhoneNumber = customer.Account.PhoneNumber;
                    existingAccount.Address = customer.Account.Address;

                    // Kiểm tra nếu mật khẩu không trống, cập nhật mật khẩu mới
                    if (!string.IsNullOrEmpty(customer.Account.Password))
                    {
                        existingAccount.Password = customer.Account.Password;
                    }

                    _context.Accounts.Update(existingAccount);

                    // Cập nhật Customer
                    var existingCustomer = await _context.Customers.FindAsync(customer.CustomerId);
                    if (existingCustomer == null)
                    {
                        return NotFound();
                    }

                    _context.Entry(existingCustomer).State = EntityState.Detached;
                    _context.Customers.Update(customer);

                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Cập nhật khách hàng thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.CustomerId))
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
            return View(customer);
        }

        // GET: Customers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .Include(c => c.Account)
                .FirstOrDefaultAsync(c => c.CustomerId == id);

            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customer = await _context.Customers
                .Include(c => c.Account)
                .FirstOrDefaultAsync(c => c.CustomerId == id);

            if (customer != null)
            {
                // Xoá Account trước
                _context.Accounts.Remove(customer.Account);

                // Sau đó xoá Customer
                _context.Customers.Remove(customer);

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Xoá khách hàng thành công!";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.CustomerId == id);
        }
    }
}
