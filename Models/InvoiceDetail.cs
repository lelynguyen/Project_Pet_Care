using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace PetCare.Models
{
    public class InvoiceDetail
    {
        public int InvoiceId { get; set; }
        public int PetId { get; set; }
        public int ServiceId { get; set; }

        // Navigation properties
        [ValidateNever]
        public virtual Invoice Invoice { get; set; }
        [ValidateNever]
        public virtual Pet Pet { get; set; }
        [ValidateNever]
        public virtual Service Service { get; set; }
    }
}
