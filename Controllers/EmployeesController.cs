using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PetCare.Models;

namespace PetCare.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly PetCareContext _context;

        public EmployeesController(PetCareContext context)
        {
            _context = context;
        }

        // GET: Employees
        public async Task<IActionResult> Index()
        {
            var employees = _context.Employees
                .Include(e => e.Account)
                .Include(e => e.Role);
            return View(await employees.ToListAsync());
        }

        // GET: Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.Account)
                .Include(e => e.Role)
                .FirstOrDefaultAsync(e => e.EmployeeId == id);

            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // GET: Employees/Create
        public IActionResult Create()
        {
            ViewBag.RoleId = new SelectList(_context.Roles, "RoleId", "RoleName");
            return View();
        }

        // POST: Employees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Employee employee)
        {
            if (ModelState.IsValid)
            {
                // Thêm Account
                _context.Accounts.Add(employee.Account);
                await _context.SaveChangesAsync();

                // Gán AccountId cho Employee
                employee.AccountId = employee.Account.AccountId;

                // Thêm Employee
                _context.Employees.Add(employee);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewBag.RoleId = new SelectList(_context.Roles, "RoleId", "RoleName", employee.RoleId);
            return View(employee);
        }

        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.Account)
                .FirstOrDefaultAsync(e => e.EmployeeId == id);

            if (employee == null)
            {
                return NotFound();
            }

            ViewBag.RoleId = new SelectList(_context.Roles, "RoleId", "RoleName", employee.RoleId);
            return View(employee);
        }

        // POST: Employees/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Employee employee)
        {
            if (id != employee.EmployeeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Lấy thông tin Account từ cơ sở dữ liệu
                    var existingAccount = await _context.Accounts.FindAsync(employee.Account.AccountId);
                    if (existingAccount == null)
                    {
                        return NotFound();
                    }

                    // Cập nhật các trường thông tin
                    existingAccount.FullName = employee.Account.FullName;
                    existingAccount.PhoneNumber = employee.Account.PhoneNumber;
                    existingAccount.Address = employee.Account.Address;

                    // Kiểm tra nếu mật khẩu không trống, cập nhật mật khẩu mới
                    if (!string.IsNullOrEmpty(employee.Account.Password))
                    {
                        existingAccount.Password = employee.Account.Password;
                    }

                    // Cập nhật Account
                    _context.Accounts.Update(existingAccount);

                    // Cập nhật Employee
                    var existingEmployee = await _context.Employees.FindAsync(employee.EmployeeId);
                    if (existingEmployee == null)
                    {
                        return NotFound();
                    }
                    existingEmployee.RoleId = employee.RoleId;

                    _context.Employees.Update(existingEmployee);

                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Cập nhật nhân viên thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.EmployeeId))
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
            ViewBag.RoleId = new SelectList(_context.Roles, "RoleId", "RoleName", employee.RoleId);
            return View(employee);
        }


        // GET: Employees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.Account)
                .Include(e => e.Role)
                .FirstOrDefaultAsync(e => e.EmployeeId == id);

            if (employee == null)
            {
                return NotFound();
            }

            // Không thể xoá nhân viên có RoleId là 1 hoặc 2
            if (employee.RoleId == 1 || employee.RoleId == 2)
            {
                TempData["ErrorMessage"] = "Không thể xoá nhân viên có vai trò quản trị hoặc quản lý.";
                return RedirectToAction(nameof(Index));
            }

            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _context.Employees
                .Include(e => e.Account)
                .FirstOrDefaultAsync(e => e.EmployeeId == id);

            if (employee != null)
            {
                // Xoá Account trước
                _context.Accounts.Remove(employee.Account);

                // Sau đó xoá Employee
                _context.Employees.Remove(employee);

                await _context.SaveChangesAsync();
            }
            TempData["SuccessMessage"] = "Xoá nhân viên thành công!";
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.EmployeeId == id);
        }
    }
}
