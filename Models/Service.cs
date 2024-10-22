using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace PetCare.Models
{
    public class Service
    {
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public decimal Price { get; set; }
        public string Note { get; set; }

        // Navigation properties
        [ValidateNever]

        public virtual ICollection<Appointment> Appointments { get; set; }
        [ValidateNever]

        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        [ValidateNever]

        public virtual ICollection<InvoiceDetail> InvoiceDetails { get; set; }
    }
}
