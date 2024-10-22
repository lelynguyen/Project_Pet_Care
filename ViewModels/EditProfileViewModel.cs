using System.ComponentModel.DataAnnotations;

namespace PetCare.ViewModels
{
    public class EditProfileViewModel
    {
        [Required(ErrorMessage = "Họ và tên không được để trống.")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Số điện thoại không được để trống.")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Địa chỉ không được để trống.")]
        public string Address { get; set; }
    }
}
