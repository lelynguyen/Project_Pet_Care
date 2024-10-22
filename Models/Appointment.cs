using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace PetCare.Models
{
    public class Appointment
    {
        public int AppointmentId { get; set; }

        [Required(ErrorMessage = "Khách hàng không được để trống.")]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Thời gian hẹn không được để trống.")]
        [DataType(DataType.DateTime)]
        public DateTime AppointmentTime { get; set; }

        [Required(ErrorMessage = "Dịch vụ không được để trống.")]
        public int ServiceId { get; set; }

        public string Note { get; set; }

        public bool Status { get; set; } // false: Chưa hoàn thành, true: Hoàn thành

        // Navigation properties
        [ValidateNever]
        public virtual Customer Customer { get; set; }
        [ValidateNever]
        public virtual Service Service { get; set; }
    }
}
