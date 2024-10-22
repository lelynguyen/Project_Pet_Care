using Microsoft.EntityFrameworkCore;
using PetCare.Models;
namespace PetCare.Models
{
    public class PetCareContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Pet> Pets { get; set; }
        public DbSet<TypePet> TypePets { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceDetail> InvoiceDetails { get; set; }
        public DbSet<Post> Posts { get; set; }

        // Constructor
        public PetCareContext(DbContextOptions<PetCareContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderDetail>()
                .HasKey(od => new { od.OrderId, od.PetId, od.ServiceId });

            modelBuilder.Entity<InvoiceDetail>()
                .HasKey(id => new { id.InvoiceId, id.PetId, id.ServiceId });

            // Cấu hình khóa ngoại để tránh nhiều đường dẫn cascade
            modelBuilder.Entity<Invoice>()
                .HasOne(i => i.Customer)
                .WithMany(c => c.Invoices)
                .HasForeignKey(i => i.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Invoice>()
                .HasOne(i => i.Employee)
                .WithMany(e => e.Invoices)
                .HasForeignKey(i => i.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Thêm các cấu hình khác nếu cần thiết
            modelBuilder.Entity<OrderDetail>()
        .HasKey(od => new { od.OrderId, od.PetId, od.ServiceId });

            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Order)
                .WithMany(o => o.OrderDetails)
                .HasForeignKey(od => od.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Pet)
                .WithMany(p => p.OrderDetails)
                .HasForeignKey(od => od.PetId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Service)
                .WithMany(s => s.OrderDetails)
                .HasForeignKey(od => od.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<InvoiceDetail>()
        .HasKey(id => new { id.InvoiceId, id.PetId, id.ServiceId });

            modelBuilder.Entity<InvoiceDetail>()
                .HasOne(id => id.Invoice)
                .WithMany(i => i.InvoiceDetails)
                .HasForeignKey(id => id.InvoiceId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<InvoiceDetail>()
                .HasOne(id => id.Pet)
                .WithMany(p => p.InvoiceDetails)
                .HasForeignKey(id => id.PetId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<InvoiceDetail>()
                .HasOne(id => id.Service)
                .WithMany(s => s.InvoiceDetails)
                .HasForeignKey(id => id.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);
        }

    }
}