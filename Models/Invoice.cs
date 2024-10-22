using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace PetCare.Models
{
    public class Invoice
    {
        public int InvoiceId { get; set; }
        public int CustomerId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal TotalPrice { get; set; }

        // Navigation properties
        [ValidateNever]
        public virtual Customer Customer { get; set; }
        [ValidateNever]
        public virtual Employee Employee { get; set; }
        [ValidateNever]
        public virtual ICollection<InvoiceDetail> InvoiceDetails { get; set; }
    }
}
