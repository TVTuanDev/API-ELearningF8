using ELearningF8.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using System.Text.Json;

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
            List<Course> courses = await _context.Courses.ToListAsync();

            return Ok(new { Status = 200, Message = "Success", Data = courses });
        }

        [HttpGet("/course/{id}")]
        public async Task<IActionResult> GetCourseById(int id)
        {
            if (id < 1) return BadRequest(new { Status = 400, Message = "Id truyền vào không hợp lệ" });
            Course? course = await _context.Courses
                .Where(c => c.Id == id)
                .Include(c => c.Chapters)
                    .ThenInclude(c => c.Lessons)
                .FirstOrDefaultAsync();

            if (course == null) return BadRequest(new { Status = 400, Message = $"Không tìm thấy khóa học có id = {id}" });

            return Ok(new { Status = 200, Message = "Success", Data = course });
        }

        [HttpGet("/lesson/{id}")]
        public async Task<IActionResult> GetLesson(int id)
        {
            if (id < 1) return BadRequest(new { Status = 400, Message = "Id truyền vào không hợp lệ" });
            Lesson? lessonDb = await _context.Lessons.FirstOrDefaultAsync(ls => ls.Id == id);

            if (lessonDb == null) return BadRequest(new { Status = 400, Message = $"Không tìm thấy khóa học có id = {id}" });

            var lessonView = new Dictionary<string, object>
            {
                {"Id", lessonDb.Id},
                {"Title", lessonDb.Title},
                {"Sort", lessonDb.Sort},
                {"Content", lessonDb.Content ?? ""},
                {"Link", lessonDb.Link ?? ""},
                {"Slug", lessonDb.Slug ?? ""},
                {"CreateAt", lessonDb.CreateAt},
                {"UpdateAt", lessonDb.UpdateAt},
                {"IdChapter", lessonDb.IdChapter},
                {"IdType", lessonDb.IdType}
            };

            if (lessonDb.IdType == 1) return Ok(new { Status = 200, Message = "Success", Data = lessonView });

            if(lessonDb.IdType == 2) return Ok(new { Status = 200, Message = "Chưa có bài thực hành" });

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

            lessonView.Remove("Content");
            lessonView.Remove("Link");

            lessonView.Add("Descriptions", questionDb.Descriptions ?? "");
            lessonView.Add("Question", questionDb.Question);
            lessonView.Add("Answers", answers);

            return Ok(new { Status = 200, Message = "Success", Data = lessonView });
        }

        [HttpGet("/course/number-user/{id}")]
        public async Task<IActionResult> GetCountUserByIdCourse(int id)
        {
            if (id < 1) return BadRequest(new { Status = 400, Message = $"Không tìm thấy theo id course = {id}" });

            var count = await _context.UserCourses.Where(uc => uc.IdCourse == id).CountAsync();

            return Ok(new { Status = 200, Message = "Success", Data = count });
        }
    }
}
