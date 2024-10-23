using Microsoft.AspNetCore.Mvc;
using PetCare.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System;
using System.Text;
using System.Security.Cryptography;
using Microsoft.AspNetCore.WebUtilities;
using PetCare.Services;

namespace PetCare.Controllers
{
    public class HomeController : Controller
    {
        private readonly PetCareContext _context;
        private readonly EmailSender _emailSender;

        public HomeController(PetCareContext context, EmailSender emailSender)
        {
            _context = context;
            _emailSender = emailSender;
        }

        // Trang chủ hiển thị form đăng nhập và đăng ký
        public IActionResult Index()
        {
            return View();
        }

        // GET: /Home/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Home/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra tài khoản trong cơ sở dữ liệu
                var account = await _context.Accounts
                    .FirstOrDefaultAsync(a => a.Email == email && a.Password == password);

                if (account != null)
                {
                    // Đăng nhập thành công, lưu thông tin vào Session hoặc Cookie
                    // Ở đây tôi sẽ sử dụng Session
                    HttpContext.Session.SetInt32("AccountId", account.AccountId);
                    HttpContext.Session.SetString("FullName", account.FullName);

                    // Chuyển hướng đến trang Dashboard hoặc trang chính
                    return RedirectToAction("Index", "Dashboard");
                }
                else
                {
                    ModelState.AddModelError("", "Email hoặc mật khẩu không đúng.");
                }
            }
            return View();
        }

        // GET: /Home/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Home/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(Account account)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra email đã tồn tại chưa
                var existingAccount = await _context.Accounts
                    .FirstOrDefaultAsync(a => a.Email == account.Email);

                if (existingAccount == null)
                {
                    // Tạo token xác nhận email
                    var token = GenerateEmailConfirmationToken(account);

                    // Tạo liên kết xác nhận email
                    var confirmationLink = Url.Action("ConfirmEmail", "Home", new { token = token }, Request.Scheme);

                    // Gửi email xác nhận (giả sử bạn có chức năng gửi email)
                    await _emailSender.SendEmailAsync(account.Email, "Email Confirmation",
                $"Please confirm your email by clicking this link: <a href='{confirmationLink}'>Confirm Email</a>");

                    // Thông báo người dùng kiểm tra email để xác nhận
                    ViewBag.Message = "Đăng ký thành công! Vui lòng kiểm tra email để xác nhận tài khoản.";
                    return View("RegisterConfirmation");
                }
                else
                {
                    ModelState.AddModelError("", "Email đã được sử dụng.");
                }
            }
            return View(account);
        }

        // GET: /Home/ConfirmEmail?email=...&code=...
        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string token)
        {
            if (token == null)
            {
                return NotFound();
            }

            // Giải mã token để lấy thông tin tài khoản
            var account = DecodeEmailConfirmationToken(token);

            if (account != null)
            {
                // Kiểm tra lại email đã tồn tại chưa
                var existingAccount = await _context.Accounts
                    .FirstOrDefaultAsync(a => a.Email == account.Email);

                if (existingAccount == null)
                {
                    // Lưu tài khoản vào cơ sở dữ liệu
                    _context.Accounts.Add(account);
                    await _context.SaveChangesAsync();

                    ViewBag.Message = "Xác nhận email thành công! Bạn có thể đăng nhập.";
                    return View("ConfirmEmailConfirmation");
                }
                else
                {
                    ViewBag.Message = "Email đã được xác nhận trước đó.";
                    return View("ConfirmEmailConfirmation");
                }
            }
            else
            {
                ViewBag.Message = "Mã xác nhận không hợp lệ.";
                return View("ConfirmEmailConfirmation");
            }
        }

        // Hàm tạo token xác nhận email
        private string GenerateEmailConfirmationToken(Account account)
        {
            // Chuyển đổi thông tin tài khoản thành chuỗi JSON
            var accountJson = Newtonsoft.Json.JsonConvert.SerializeObject(account);

            // Mã hóa chuỗi JSON
            var accountBytes = Encoding.UTF8.GetBytes(accountJson);
            var token = WebEncoders.Base64UrlEncode(accountBytes);

            return token;
        }

        // Hàm giải mã token xác nhận email
        private Account DecodeEmailConfirmationToken(string token)
        {
            try
            {
                var accountBytes = WebEncoders.Base64UrlDecode(token);
                var accountJson = Encoding.UTF8.GetString(accountBytes);
                var account = Newtonsoft.Json.JsonConvert.DeserializeObject<Account>(accountJson);
                return account;
            }
            catch
            {
                return null;
            }
        }

        // GET: /Home/Logout
        [HttpGet]
        public IActionResult Logout()
        {
            // Xóa Session hoặc Cookie
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
