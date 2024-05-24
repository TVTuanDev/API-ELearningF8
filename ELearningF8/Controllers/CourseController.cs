using ELearningF8.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using System.Text.Json;
using ELearningF8.Attributes;
using System.Security.Claims;
using ELearningF8.ViewModel.Course;
using CloudinaryDotNet;
using ELearningF8.Models;
using ELearningF8.Utilities;
using CloudinaryDotNet.Actions;

namespace ELearningF8.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CourseController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("/course")]
        public async Task<IActionResult> GetCourses()
        {
            List<Course> courses = await _context.Courses
                .Include(c => c.Chapters)
                    .ThenInclude(ch => ch.Lessons)
                .OrderByDescending(c => c.CreateAt)
                .ToListAsync();

            return Ok(new { Status = 200, Message = "Success", Data = courses });
        }

        [HttpGet("/course/{slug}")]
        public async Task<IActionResult> GetCourseById(string slug)
        {
            if (string.IsNullOrEmpty(slug)) return BadRequest(new { Status = 400, Message = "Slug truyền vào không hợp lệ" });
            Course? course = await _context.Courses
                .Where(c => c.Slug == slug)
                .Include(c => c.Chapters)
                    .ThenInclude(ch => ch.Lessons)
                .FirstOrDefaultAsync();

            if (course == null) return NotFound(new { Status = 400, Message = $"Không tìm thấy khóa học có slug = {slug}" });

            return Ok(new { Status = 200, Message = "Success", Data = course });
        }
        
        [HttpGet("/course/number-user/{id}")]
        public async Task<IActionResult> GetCountUserByIdCourse(int id)
        {
            if (id < 1) return NotFound(new { Status = 400, Message = $"Không tìm thấy theo id course = {id}" });

            var count = await _context.UserCourses.Where(uc => uc.IdCourse == id).CountAsync();

            return Ok(new { Status = 200, Message = "Success", Data = count });
        }

        [HttpPost("/course/register-user")]
        [JwtAuthorize]
        public async Task<IActionResult> UserCourse(UserCourseVM model)
        {
            try
            {
                int.TryParse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier), out int idUser);
                var user = await _context.Users.FindAsync(idUser);
                if (user is null) return NotFound(new { Status = 404, Message = "Không tìm thấy user" });

                var course = await _context.Courses.FindAsync(model.IdCourse);
                if (course is null) return NotFound(new { Status = 404, Message = "Không tìm thấy course" });

                var userCourse = new UserCourse
                {
                    IdUser = idUser,
                    IdCourse = model.IdCourse
                };

                _context.UserCourses.Add(userCourse);
                await _context.SaveChangesAsync();

                return Ok(new { Status = 200, Message = "Success" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Status = 400, Message = ex.Message });
            }
        }

        [HttpGet("/course/payments")]
        [JwtAuthorize]
        public async Task<IActionResult> GetPaymentsCourse(string slug)
        {
            var course = await _context.Courses.FirstOrDefaultAsync(c => c.Slug == slug);
            if (course is null)
                return NotFound(new { Status = 404, Message = "Không tìm thấy khóa học" });

            //int.TryParse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier), out int idUser);
            //var user = await _context.Users.FindAsync(idUser);
            //if (user is null)
            //    return NotFound(new { Status = 404, Message = "Không tìm thấy người dùng" });

            string codePayments = RandomGenerator.RandomCode(6);

            var urlPayments = 
                $"https://qr.sepay.vn/img?acc={PaymentsCourse.AccountNumber}" +
                $"&bank={PaymentsCourse.Bank}" +
                $"&amount={Convert.ToInt32(course.Discount)}" +
                $"&des={codePayments}";

            var payments = new
            {
                course.Title,
                course.Price,
                course.Discount,
                PaymentsCourse.AccountNumber,
                PaymentsCourse.AccountName,
                content = codePayments,
                PaymentsCourse.Branch,
                qrCode = urlPayments
            };
            
            return Ok(new { Status = 200, Message = "Success", Data = payments });
        }

        [HttpGet("/lesson")]
        [JwtAuthorize]
        public async Task<IActionResult> GetLesson(string slugCourse, int id)
        {
            if (id < 1) return BadRequest(new { Status = 400, Message = "Id truyền vào không hợp lệ" });
            var lessonDb = await _context.Lessons.FirstOrDefaultAsync(ls => ls.Id == id);

            if (lessonDb == null) return NotFound(new { Status = 400, Message = $"Không tìm thấy khóa học có id = {id}" });

            var course = _context.Courses.Include(c => c.Chapters).FirstOrDefault(c => c.Slug == slugCourse);

            if (course == null) return BadRequest(new { Status = 400, Message = "Không tìm thấy course" });

            var chapter = course.Chapters.FirstOrDefault(ct => ct.Id == lessonDb.IdChapter);
            if (chapter == null) return BadRequest(new { Status = 400, Message = "Lesson không tồn tại trong course" });

            int.TryParse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier), out int idUser);

            var result = _context.UserCourses.FirstOrDefault(uc => uc.IdUser == idUser & uc.IdCourse == course.Id);

            if (result == null) return BadRequest(new { Status = 400, Message = "User chưa đăng ký khóa học này" });

            var lessonView = new Dictionary<string, object>
            {
                {"id", lessonDb.Id},
                {"title", lessonDb.Title},
                {"sort", lessonDb.Sort},
                {"content", lessonDb.Content ?? ""},
                {"link", lessonDb.Link ?? ""},
                {"slug", lessonDb.Slug ?? ""},
                {"createAt", lessonDb.CreateAt},
                {"updateAt", lessonDb.UpdateAt},
                {"idChapter", lessonDb.IdChapter},
                {"idType", lessonDb.IdType}
            };

            if (lessonDb.IdType == 1) return Ok(new { Status = 200, Message = "Success", Data = lessonView });

            if (lessonDb.IdType == 2) return Ok(new { Status = 200, Message = "Chưa có bài thực hành" });

            QuestionLesson? questionDb = await _context.QuestionLessons
                .Where(q => q.IdLesson == lessonDb.Id).Include(q => q.Answers).FirstAsync();

            var answers = questionDb.Answers.Select(a => new
            {
                a.Id,
                a.AnswerQuestion,
                a.Status,
                a.Explain,
                a.IdQuestion
            });

            lessonView.Remove("content");
            lessonView.Remove("link");

            lessonView.Add("descriptions", questionDb.Descriptions ?? "");
            lessonView.Add("question", questionDb.Question);
            lessonView.Add("answers", answers);

            return Ok(new { Status = 200, Message = "Success", Data = lessonView });
        }
    }
}
