using ELearningF8.Data;
using MailKit.Security;
using MimeKit.Text;
using MimeKit;
using MailKit.Net.Smtp;

namespace ELearningF8.Services
{
    public class SendMailServices
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _conf;
        private readonly AppDbContext _context;

        public SendMailServices
            (
            ILogger<SendMailServices> logger,
            IConfiguration conf,
            AppDbContext context
            )
        {
            _logger = logger;
            _conf = conf.GetSection("MailSettings");
            _context = context;
        }

        public async Task SendMailAsync(string emailTo, string subject, string htmlMessage)
        {
            var email = new MimeMessage();

            email.From.Add(MailboxAddress.Parse(_conf["Mail"]));
            email.To.Add(MailboxAddress.Parse(emailTo));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = htmlMessage };

            using var smtp = new SmtpClient();

            try
            {
                smtp.Connect(_conf["Host"], int.Parse(_conf["Port"] ?? ""), SecureSocketOptions.StartTls);
                smtp.Authenticate(_conf["Mail"], _conf["Password"]);
                await smtp.SendAsync(email);
            }
            catch (Exception ex)
            {
                // Gửi mail thất bại, nội dung email sẽ lưu vào thư mục mailssave
                System.IO.Directory.CreateDirectory("mailssave");
                var emailsavefile = string.Format(@"mailssave/{0}.eml", Guid.NewGuid());
                await email.WriteToAsync(emailsavefile);

                _logger.LogInformation("Lỗi gửi mail, lưu tại - " + emailsavefile);
                _logger.LogError(ex.Message);
            }

            smtp.Disconnect(true);
        }

        public Task SendSmsAsync(string number, string message)
        {
            // Cài đặt dịch vụ gửi SMS tại đây
            System.IO.Directory.CreateDirectory("smssave");
            var emailsavefile = string.Format(@"smssave/{0}-{1}.txt", number, Guid.NewGuid());
            System.IO.File.WriteAllTextAsync(emailsavefile, message);
            return Task.FromResult(0);
        }
    }
}
