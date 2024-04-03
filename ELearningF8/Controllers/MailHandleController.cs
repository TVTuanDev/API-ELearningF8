using ELearningF8.Data;
using ELearningF8.Models;
using ELearningF8.Services;
using ELearningF8.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Text;
using System.Text.RegularExpressions;

namespace ELearningF8.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MailHandleController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly SendMailServices _mailService;
        private readonly RandomGenerator _random;
        private readonly IMemoryCache _cache;

        public MailHandleController
            (
            AppDbContext context,
            SendMailServices mailService, 
            RandomGenerator random, IMemoryCache cache
            )
        {
            _context = context;
            _mailService = mailService;
            _random = random;
            _cache = cache;
        }

        [HttpPost("/send-mail/register")]
        public async Task<IActionResult> SendMailRegister(MailVM model)
        {
            if(!CheckRegexMail(model.Email)) 
                return BadRequest(new { Status = 400, Message = "Không đúng định dạng email" });

            if (await GetUserByEmail(model.Email) != null) 
                return BadRequest(new { Status = 400,  Message = "Email đã được sử dụng" });
            try
            {
                var code = _random.RandomCode();
                var htmlMessage = $@"<h3>Bạn đã đăng ký tài khoản trên F8</h3>
                    <p>Tiếp tục đăng ký với F8 bằng cách nhập mã bên dưới:</p>
                    <h1>{code}</h1>
                    <p>Mã xác minh sẽ hết hạn sau 10 phút.</p>
                    <p><b>Nếu bạn không yêu cầu mã,</b> bạn có thể bỏ qua tin nhắn này.</p>";

                await SendCodeAsync(model.Email, "Fullstack", code, htmlMessage);

                return Ok(new { Status = 200, Message = "Success", Data = code });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Status = 400, Message = ex.Message });
            }
        }

        [HttpPost("/send-mail/login")]
        public async Task<IActionResult> SendMailLogin(MailVM model)
        {
            if (!CheckRegexMail(model.Email))
                return BadRequest(new { Status = 400, Message = "Không đúng định dạng email" });

            if (await GetUserByEmail(model.Email) == null) 
                return BadRequest(new { Status = 400,Message = "Email chưa được đăng ký" });
            try
            {
                var code = _random.RandomCode();
                var htmlMessage = $@"<h3>Bạn đang đăng nhập tài khoản trên F8</h3>
                    <p>Tiếp tục đăng nhập với F8 bằng cách nhập mã bên dưới:</p>
                    <h1>{code}</h1>
                    <p>Mã xác minh sẽ hết hạn sau 10 phút.</p>
                    <p><b>Nếu bạn không yêu cầu mã,</b> bạn có thể bỏ qua tin nhắn này.</p>";

                await SendCodeAsync(model.Email, "Fullstack", code, htmlMessage);

                return Ok(new { Status = 200, Message = "Success", Data = code });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Status = 400, Message = ex.Message });
            }
        }

        [HttpPost("/send-mail/forgot-password")]
        public async Task<IActionResult> SendMailForgotPassword(MailVM model)
        {
            if (!CheckRegexMail(model.Email))
                return BadRequest(new { Status = 400, Message = "Không đúng định dạng email" });

            if (await GetUserByEmail(model.Email) == null)
                return BadRequest(new { Status = 400, Message = "Email chưa được đăng ký" });
            try
            {
                var code = _random.RandomCode();
                var htmlMessage = $@"<h3>Bạn quên mật khẩu đăng nhập trên F8</h3>
                    <p>Lấy lại mật khẩu với F8 bằng cách nhập mã bên dưới:</p>
                    <h1>{code}</h1>
                    <p>Mã xác minh sẽ hết hạn sau 10 phút.</p>
                    <p><b>Nếu bạn không yêu cầu mã,</b> bạn có thể bỏ qua tin nhắn này.</p>";

                await SendCodeAsync(model.Email, "Fullstack", code, htmlMessage);

                return Ok(new { Status = 200, Message = "Success", Data = code });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Status = 400, Message = ex.Message });
            }
        }

        [NonAction]
        public async Task SendCodeAsync(string email, string subject, string code, string htmlMessage)
        {
            await _mailService.SendMailAsync(email, subject, htmlMessage);

            MemoryCacheEntryOptions options = new MemoryCacheEntryOptions();
            options.AbsoluteExpiration = DateTime.UtcNow.AddMinutes(10);

            _cache.Set<string>(email, code, options);   
        }

        [NonAction]
        public bool CheckRegexMail(string email)
        {
            string pattern = @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$";
            Regex regex = new Regex(pattern);
            if (regex.IsMatch(email)) return true;
            return false;
        }

        [NonAction]
        public async Task<User?> GetUserByEmail(string email, string proverder = "email")
        {
            var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == email && u.Providers == proverder);
            if (user != null) return user;
            return null;
        }
    }
}
