using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetCare.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PetCare.Controllers
{
    public class PostsController : Controller
    {
        private readonly PetCareContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public PostsController(PetCareContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        // GET: Posts
        public async Task<IActionResult> Index(string searchString, string sortOrder)
        {
            var posts = _context.Posts.Include(p => p.Employee).AsQueryable();

            // Tìm kiếm theo tiêu đề
            if (!string.IsNullOrEmpty(searchString))
            {
                posts = posts.Where(p => p.Title.Contains(searchString));
            }

            // Sắp xếp theo thời gian
            switch (sortOrder)
            {
                case "time_desc":
                    posts = posts.OrderByDescending(p => p.PostedTime);
                    break;
                default:
                    posts = posts.OrderBy(p => p.PostedTime);
                    break;
            }

            // Chuẩn bị dữ liệu cho sắp xếp
            var sortOptions = new List<SelectListItem>
    {
        new SelectListItem { Text = "Cũ nhất", Value = "", Selected = string.IsNullOrEmpty(sortOrder) },
        new SelectListItem { Text = "Mới nhất", Value = "time_desc", Selected = sortOrder == "time_desc" }
    };
            ViewBag.SortOptions = sortOptions;

            // Truyền lại giá trị tìm kiếm để hiển thị trong View
            ViewBag.SearchString = searchString;

            return View(await posts.ToListAsync());
        }


        // GET: Posts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Employee)
                .FirstOrDefaultAsync(m => m.PostId == id);

            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // GET: Posts/Create
        public IActionResult Create()
        {
            // Nếu cần chọn Employee, bạn có thể truyền dữ liệu qua ViewBag hoặc ViewData
            // Ở đây giả sử EmployeeId được đặt trong action
            return View();
        }

        // POST: Posts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Post post, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                // Thiết lập thời gian đăng và cập nhật
                post.PostedTime = DateTime.Now;
                post.UpdatedTime = DateTime.Now;

                // Xử lý upload hình ảnh
                if (imageFile != null && imageFile.Length > 0)
                {
                    // Lưu hình ảnh vào thư mục wwwroot/images/posts
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    string fileName = Path.GetFileNameWithoutExtension(imageFile.FileName);
                    string extension = Path.GetExtension(imageFile.FileName);
                    fileName = fileName + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + extension;
                    string path = Path.Combine(wwwRootPath + "/images/posts/", fileName);

                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(fileStream);
                    }

                    // Lưu đường dẫn hình ảnh vào thuộc tính Image
                    post.Image = "/images/posts/" + fileName;
                }

                // Thiết lập EmployeeId, ví dụ từ người dùng đăng nhập hiện tại
                // post.EmployeeId = currentEmployeeId;

                _context.Add(post);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Thêm bài đăng thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View(post);
        }

        // GET: Posts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            return View(post);
        }

        // POST: Posts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Post post, IFormFile imageFile)
        {
            if (id != post.PostId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Thiết lập thời gian cập nhật
                    post.UpdatedTime = DateTime.Now;

                    // Xử lý upload hình ảnh
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        // Xóa hình ảnh cũ nếu tồn tại
                        if (!string.IsNullOrEmpty(post.Image))
                        {
                            string oldImagePath = Path.Combine(_hostEnvironment.WebRootPath, post.Image.TrimStart('/'));
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        // Lưu hình ảnh mới
                        string wwwRootPath = _hostEnvironment.WebRootPath;
                        string fileName = Path.GetFileNameWithoutExtension(imageFile.FileName);
                        string extension = Path.GetExtension(imageFile.FileName);
                        fileName = fileName + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + extension;
                        string path = Path.Combine(wwwRootPath + "/images/posts/", fileName);

                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(fileStream);
                        }

                        // Lưu đường dẫn hình ảnh vào thuộc tính Image
                        post.Image = "/images/posts/" + fileName;
                    }

                    _context.Update(post);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Cập nhật bài đăng thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.PostId))
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
            return View(post);
        }

        // GET: Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Employee)
                .FirstOrDefaultAsync(m => m.PostId == id);

            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Posts.FindAsync(id);

            // Xóa hình ảnh nếu tồn tại
            if (!string.IsNullOrEmpty(post.Image))
            {
                string imagePath = Path.Combine(_hostEnvironment.WebRootPath, post.Image.TrimStart('/'));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Xóa bài đăng thành công!";
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.PostId == id);
        }
    }
}
