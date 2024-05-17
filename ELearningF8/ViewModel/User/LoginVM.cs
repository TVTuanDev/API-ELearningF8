using System.ComponentModel.DataAnnotations;

namespace ELearningF8.ViewModel.User
{
    public class LoginVM
    {
        [Required(ErrorMessage = "Email không hợp lệ")]
        [EmailAddress(ErrorMessage = "Vui lòng nhập đúng định dạng email")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Mật khẩu không hợp lệ")]
        [MinLength(8, ErrorMessage = "Mật khẩu tối thiểu là {1} ký tự")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;
    }
}
