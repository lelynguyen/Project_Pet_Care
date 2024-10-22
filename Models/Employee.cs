using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace PetCare.Models
{
    public class Employee
    {
        public int EmployeeId { get; set; }
        public int RoleId { get; set; }
        public int AccountId { get; set; }

        // Navigation properties
        [ValidateNever]
        public virtual Role Role { get; set; }

        [ValidateNever]
        public virtual Account Account { get; set; }

        [ValidateNever]
        public virtual ICollection<Invoice> Invoices { get; set; }

        [ValidateNever]
        public virtual ICollection<Post> Posts { get; set; }
    }
}
