using Microsoft.EntityFrameworkCore;
using PetCare.Models;
using PetCare.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<EmailSender>();

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure DbContext with the connection string
builder.Services.AddDbContext<PetCareContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Cấu hình Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Sử dụng Session
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
