using System.ComponentModel.DataAnnotations;

namespace ELearningF8.ViewModel.User
{
    public class ForgotPasswordVM
    {
        [Required(ErrorMessage = "Email bắt buộc nhập")]
        [EmailAddress(ErrorMessage = "Vui lòng nhập đúng định dạng email")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Email không hợp lệ")]
        public string Code { get; set; }
    }
}
