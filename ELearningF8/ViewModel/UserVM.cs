using System.ComponentModel.DataAnnotations;

namespace ELearningF8.ViewModel
{
    public class UserVM
    {
        [Display(Name = "id")]
        [Required(ErrorMessage = "Bắt buộc nhập {0}")]
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

        public string? Phone { get; set; }

        public string Avatar { get; set; } = "https://res.cloudinary.com/daeiiokje/image/upload/v1710573648/ELearningF8/Images/avartar%20default_638461704460223693.jpg";

        public string BgAvatar { get; set; } = "http://res.cloudinary.com/daeiiokje/image/upload/v1710606891/ELearningF8/Images/bg-avatar_638462036889615906.png";

        public string Status { get; set; } = "active";

        public bool TwoFactorEnabled { get; set; }

        public DateTime UpdateAt { get; set; } = DateTime.UtcNow;
    }
}
