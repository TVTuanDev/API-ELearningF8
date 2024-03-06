using System.ComponentModel.DataAnnotations;

namespace ELearningF8.ViewModel
{
    public class LoginVM
    {
        [Required(ErrorMessage = "Email không hợp lệ")]
        [EmailAddress(ErrorMessage = "Vui lòng nhập đúng định dạng email")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Mật khẩu không hợp lệ")]
        [StringLength(12, MinimumLength = 6, ErrorMessage = "Mật khẩu tối thiểu {2} đến {1} ký tự")]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)[a-zA-Z\\d]{6,12}$", ErrorMessage = "Mật khẩu phải có chữ hoa, chữ thường, số")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "Code không hợp lệ")]
        [StringLength(8, MinimumLength = 8, ErrorMessage = "Code không hợp lệ")]
        public string Code { get; set; } = null!;
    }
}
