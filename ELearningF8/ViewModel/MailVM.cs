using System.ComponentModel.DataAnnotations;

namespace ELearningF8.ViewModel
{
    public class MailVM
    {
        [Required(ErrorMessage = "Email bắt buộc nhập")]
        [EmailAddress(ErrorMessage = "Vui lòng nhập đúng định dạng email")]
        public string Email { get; set; } = null!;
    }
}
