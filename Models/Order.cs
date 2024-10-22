using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace PetCare.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime PaymentDate { get; set; }

        // Navigation properties
        [ValidateNever]
        public virtual Customer Customer { get; set; }
        [ValidateNever]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
