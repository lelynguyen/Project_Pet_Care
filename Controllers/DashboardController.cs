using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetCare.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace PetCare.Controllers
{
    public class DashboardController : Controller
    {
        private readonly PetCareContext _context;

        public DashboardController(PetCareContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Thống kê số lượng khách hàng
            var customerCount = await _context.Customers.CountAsync();

            // Thống kê số lượng thú cưng
            var petCount = await _context.Pets.CountAsync();

            // Truyền dữ liệu sang View bằng ViewBag
            ViewBag.CustomerCount = customerCount;
            ViewBag.PetCount = petCount;

            // Lấy ngày hiện tại
            DateTime today = DateTime.Today;

            // Lấy ngày đầu tiên của tháng hiện tại
            DateTime startOfCurrentMonth = new DateTime(today.Year, today.Month, 1);

            // Lấy ngày đầu tiên của tháng cách đây 3 tháng (tổng cộng 4 tháng)
            DateTime startDate = startOfCurrentMonth.AddMonths(-3);

            // Lấy ngày cuối cùng của tháng hiện tại
            DateTime endDate = startOfCurrentMonth.AddMonths(1).AddDays(-1);

            // Biểu đồ số lượng lịch hẹn trong 4 tháng gần đây
            var appointments = await _context.Appointments
                .Where(a => a.AppointmentTime >= startDate && a.AppointmentTime <= endDate)
                .GroupBy(a => new { Year = a.AppointmentTime.Year, Month = a.AppointmentTime.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Count = g.Count()
                })
                .ToListAsync();

            // Tạo danh sách các tháng trong 4 tháng gần đây
            var months = Enumerable.Range(0, 4)
                .Select(i => startDate.AddMonths(i))
                .ToList();

            // Chuẩn bị dữ liệu cho biểu đồ lịch hẹn
            var appointmentData = months.Select(m => new
            {
                Month = m.ToString("MM/yyyy"),
                Count = appointments.FirstOrDefault(a => a.Year == m.Year && a.Month == m.Month)?.Count ?? 0
            }).ToList();

            ViewBag.AppointmentData = appointmentData;

            // Biểu đồ tỷ lệ trạng thái hoàn thành và chưa hoàn thành của lịch hẹn
            var appointmentStatus = await _context.Appointments
                .GroupBy(a => a.Status)
                .Select(g => new
                {
                    Status = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            // Chuẩn bị dữ liệu cho biểu đồ trạng thái lịch hẹn
            var statusLabels = appointmentStatus.Select(s => s.Status ? "Hoàn thành" : "Chưa hoàn thành").ToList();
            var statusCounts = appointmentStatus.Select(s => s.Count).ToList();

            ViewBag.AppointmentStatusLabels = statusLabels;
            ViewBag.AppointmentStatusCounts = statusCounts;

            // Biểu đồ số lượng bài đăng trong 3 tháng gần đây và số lượng bài bị ẩn
            DateTime startDatePosts = startOfCurrentMonth.AddMonths(-2); // Lấy từ 2 tháng trước (tổng cộng 3 tháng)

            var posts = await _context.Posts
                .Where(p => p.PostedTime >= startDatePosts && p.PostedTime <= endDate)
                .GroupBy(p => new { Year = p.PostedTime.Year, Month = p.PostedTime.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    TotalPosts = g.Count(),
                    HiddenPosts = g.Count(p => !p.Status) // Bài bị ẩn có Status = false
                })
                .ToListAsync();

            // Tạo danh sách các tháng trong 3 tháng gần đây
            var postMonths = Enumerable.Range(0, 3)
                .Select(i => startDatePosts.AddMonths(i))
                .ToList();

            // Chuẩn bị dữ liệu cho biểu đồ bài đăng
            var postData = postMonths.Select(m => new
            {
                Month = m.ToString("MM/yyyy"),
                TotalPosts = posts.FirstOrDefault(p => p.Year == m.Year && p.Month == m.Month)?.TotalPosts ?? 0,
                HiddenPosts = posts.FirstOrDefault(p => p.Year == m.Year && p.Month == m.Month)?.HiddenPosts ?? 0
            }).ToList();

            ViewBag.PostData = postData;

            return View();
        }
    }
}
