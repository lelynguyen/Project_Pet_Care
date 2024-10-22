using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
namespace PetCare.Models
{
    public class Role
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }

        // Navigation properties
        [ValidateNever]
        public virtual ICollection<Employee> Employees { get; set; }
    }
}
