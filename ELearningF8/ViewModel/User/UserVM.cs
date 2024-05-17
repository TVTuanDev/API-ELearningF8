using System.ComponentModel.DataAnnotations;

namespace ELearningF8.ViewModel.User
{
    public class UserVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên của bạn không hợp lệ")]
        public string UserName { get; set; } = null!;

        [Required(ErrorMessage = "Email không hợp lệ")]
        [EmailAddress(ErrorMessage = "Vui lòng nhập đúng định dạng email")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Mật khẩu không hợp lệ")]
        [MinLength(8, ErrorMessage = "Mật khẩu tối thiểu là {1} ký tự")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        public string? Bio { get; set; }

        public string? Avatar { get; set; }
        public string? BgAvatar { get; set; }

        public string Status { get; set; } = "active";

        public bool TwoFactorEnabled { get; set; } = false;
        public string Type { get; set; } = "guest";
    }
}
