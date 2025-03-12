using System.ComponentModel.DataAnnotations;

namespace ASMHT_PH53971.Models.ViewModels
{
    public class UserEditViewModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "Họ không được để trống.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Tên không được để trống.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email không được để trống.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        public string Email { get; set; }

        public DateTime? DateOfBirth { get; set; }
        public string Address { get; set; }
        public string Gender { get; set; }
        public string Nationality { get; set; }

        // Mật khẩu
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("ConfirmPassword", ErrorMessage = "Mật khẩu xác nhận không trùng khớp.")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
        public bool HasPassword { get; set; }
    }
}
