using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace PetCare.Models
{
    public class TypePet
    {
        public int TypePetId { get; set; }
        public string TypeName { get; set; }

        // Navigation properties
        [ValidateNever]
        public virtual ICollection<Pet> Pets { get; set; }
    }
}
