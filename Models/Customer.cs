using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace PetCare.Models
{
    public class Customer
    {
        public int CustomerId { get; set; }
        public int AccountId { get; set; }

        // Navigation properties
        [ValidateNever]
        public virtual Account Account { get; set; }
        [ValidateNever]
        public virtual ICollection<Pet> Pets { get; set; }
        [ValidateNever]
        public virtual ICollection<Order> Orders { get; set; }
        [ValidateNever]
        public virtual ICollection<Invoice> Invoices { get; set; }
    }
}
