using System.ComponentModel.DataAnnotations;

namespace ASMHT_PH53971.Models.ViewModels
{
    public class CreateUserViewModel
    {

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Mật khẩu và xác nhận mật khẩu không khớp.")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string? Address { get; set; }

        public string? Gender { get; set; }

        public string? Nationality { get; set; }

        
        [Required]
        public string Role { get; set; } = "KhachHang"; 
    }
}
