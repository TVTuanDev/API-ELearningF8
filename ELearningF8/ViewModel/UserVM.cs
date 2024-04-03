using System.ComponentModel.DataAnnotations;

namespace ELearningF8.ViewModel
{
    public class UserVM
    {
        [Display(Name = "id")]
        [Required(ErrorMessage = "Bắt buộc nhập {0}")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên của bạn")]
        public string UserName { get; set; } = null!;

        [DataType(DataType.Password)]
        public string? Password { get; set; }

        public string? Bio { get; set; }

        public IFormFile? Avatar { get; set; }

        public IFormFile? BgAvatar { get; set; }

        public string Status { get; set; } = "active";

        public bool TwoFactorEnabled { get; set; } = false;
    }
}
