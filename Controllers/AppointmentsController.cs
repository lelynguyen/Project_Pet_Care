using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PetCare.Models;

namespace PetCare.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly PetCareContext _context;

        public AppointmentsController(PetCareContext context)
        {
            _context = context;
        }

        // GET: Customers 
        [HttpGet]
        public IActionResult GetCustomers(string term)
        {
            var customersQuery = _context.Customers.Include(c => c.Account).AsQueryable();

            if (!string.IsNullOrEmpty(term))
            {
                customersQuery = customersQuery.Where(c => c.Account.FullName.Contains(term) || c.Account.PhoneNumber.Contains(term));
            }

            var customers = customersQuery
                .Select(c => new
                {
                    c.CustomerId,
                    c.Account.FullName,
                    c.Account.PhoneNumber
                })
                .Take(10)
                .ToList();

            return Json(customers);
        }



        // GET: Appointments
        public async Task<IActionResult> Index(string searchString, int? serviceId, bool? statusFilter)
        {
            var appointments = _context.Appointments
                .Include(a => a.Customer)
                    .ThenInclude(c => c.Account)
                .Include(a => a.Service)
                .AsQueryable();

            // Lọc theo tên khách hàng
            if (!string.IsNullOrEmpty(searchString))
            {
                appointments = appointments.Where(a => a.Customer.Account.FullName.Contains(searchString));
            }

            // Lọc theo dịch vụ
            if (serviceId.HasValue)
            {
                appointments = appointments.Where(a => a.ServiceId == serviceId.Value);
            }

            // Lọc theo trạng thái
            if (statusFilter.HasValue)
            {
                appointments = appointments.Where(a => a.Status == statusFilter.Value);
            }

            // Chuẩn bị dữ liệu cho bộ lọc
            ViewBag.Services = new SelectList(_context.Services, "ServiceId", "ServiceName", serviceId);

            ViewBag.StatusList = new SelectList(new List<SelectListItem>
    {
        new SelectListItem { Text = "Chưa hoàn thành", Value = "false" },
        new SelectListItem { Text = "Hoàn thành", Value = "true" }
    }, "Value", "Text", statusFilter.HasValue ? statusFilter.Value.ToString().ToLower() : null);

            return View(await appointments.ToListAsync());
        }


        // GET: Appointments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                .Include(a => a.Customer)
                    .ThenInclude(c => c.Account)
                .Include(a => a.Service)
                .FirstOrDefaultAsync(m => m.AppointmentId == id);

            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        // GET: Appointments/Create
        public IActionResult Create()
        {
            // Danh sách dịch vụ
            ViewBag.ServiceId = new SelectList(_context.Services, "ServiceId", "ServiceName");

            /*// Danh sách khách hàng (sử dụng ViewModel)
            var customers = _context.Customers.Include(c => c.Account)
                .Select(c => new CustomerViewModel
                {
                    CustomerId = c.CustomerId,
                    FullName = c.Account.FullName,
                    PhoneNumber = c.Account.PhoneNumber
                })
                .ToList();

            ViewBag.Customers = customers;*/

            return View();
        }


        // POST: Appointments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Appointment appointment)
        {
            ModelState.Remove("Status"); // Bỏ qua xác thực cho Status

            if (appointment.CustomerId == 0)
            {
                ModelState.AddModelError("CustomerId", "Vui lòng chọn khách hàng.");
            }

            if (ModelState.IsValid)
            {
                // Đảm bảo thời gian hẹn trong khoảng từ 7h00 đến 18h00
                var appointmentTime = appointment.AppointmentTime.TimeOfDay;
                if (appointmentTime < TimeSpan.FromHours(7) || appointmentTime > TimeSpan.FromHours(18))
                {
                    ModelState.AddModelError("AppointmentTime", "Thời gian hẹn phải trong khoảng từ 7h00 đến 18h00.");
                }
                else
                {
                    // Mặc định Status là false (chưa hoàn thành)
                    appointment.Status = false;

                    _context.Add(appointment);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Tạo cuộc hẹn thành công!";
                    return RedirectToAction(nameof(Index));
                }
            }

            // Nếu ModelState không hợp lệ, truyền lại ViewBag.ServiceId
            ViewBag.ServiceId = new SelectList(_context.Services, "ServiceId", "ServiceName", appointment.ServiceId);

            return View(appointment);
        }





        // GET: Appointments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                .Include(a => a.Customer)
                    .ThenInclude(c => c.Account)
                .Include(a => a.Service)
                .FirstOrDefaultAsync(m => m.AppointmentId == id);

            if (appointment == null)
            {
                return NotFound();
            }

            ViewBag.ServiceId = new SelectList(_context.Services, "ServiceId", "ServiceName", appointment.ServiceId);
            return View(appointment);
        }

        // POST: Appointments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Appointment appointment)
        {
            if (id != appointment.AppointmentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Đảm bảo thời gian hẹn trong khoảng từ 7h00 đến 18h00
                    var appointmentTime = appointment.AppointmentTime.TimeOfDay;
                    if (appointmentTime < TimeSpan.FromHours(7) || appointmentTime > TimeSpan.FromHours(18))
                    {
                        ModelState.AddModelError("AppointmentTime", "Thời gian hẹn phải trong khoảng từ 7h00 đến 18h00.");
                        ViewBag.ServiceId = new SelectList(_context.Services, "ServiceId", "ServiceName", appointment.ServiceId);
                        return View(appointment);
                    }

                    _context.Update(appointment);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Cập nhật cuộc hẹn thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppointmentExists(appointment.AppointmentId))
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
            ViewBag.ServiceId = new SelectList(_context.Services, "ServiceId", "ServiceName", appointment.ServiceId);
            return View(appointment);
        }

        // GET: Appointments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                .Include(a => a.Customer)
                    .ThenInclude(c => c.Account)
                .Include(a => a.Service)
                .FirstOrDefaultAsync(m => m.AppointmentId == id);

            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        // POST: Appointments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                _context.Appointments.Remove(appointment);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Xoá cuộc hẹn thành công!";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool AppointmentExists(int id)
        {
            return _context.Appointments.Any(e => e.AppointmentId == id);
        }
    }
}
