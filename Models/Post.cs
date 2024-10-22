using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace PetCare.Models
{
    public class Post
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
        public DateTime PostedTime { get; set; }
        public DateTime UpdatedTime { get; set; }
        public int EmployeeId { get; set; }
        public bool Status { get; set; }
        public string Image { get; set; }

        // Navigation properties
        [ValidateNever]
        public virtual Employee Employee { get; set; }
    }
}
