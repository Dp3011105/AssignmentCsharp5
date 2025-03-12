using System.ComponentModel.DataAnnotations;

namespace ASMHT_PH53971.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "Tên đăng nhập")]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Xác nhận mật khẩu")]
        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Họ không được để trống.")]
        [Display(Name = "Họ")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Tên không được để trống.")]
        [Display(Name = "Tên")]
        public string LastName { get; set; }


        public DateTime? DateOfBirth { get; set; }
        public string? Address { get; set; }
        public string? Gender { get; set; }
        public string? Nationality { get; set; }
    }
}
