using ELearningF8.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;

namespace ELearningF8.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("/home")]
        public async Task<IActionResult> Index()
        {
            var banners = await _context.Banners.ToListAsync();
            var coursesDb = await _context.Courses.ToListAsync();
            var postsDb = await _context.Posts.ToListAsync();
            var videos = await _context.Videos.ToListAsync();

            var courses = new List<object>();

            foreach(var course in coursesDb)
            {
                var totalUser = await _context.UserCourses.Where(uc => uc.IdCourse == course.Id).CountAsync();
                var courseView = new Dictionary<string, object>
                {
                    {"id", course.Id},
                    {"title", course.Title},
                    {"avatar", course.Avatar},
                    {"descriptions", course.Descriptions ?? ""},
                    {"content", course.Content ?? ""},
                    {"slug", course.Slug ?? ""},
                    {"typeCourse", course.TypeCourse},
                    {"price", course.Price},
                    {"discount", course.Discount},
                    {"isComing", course.IsComing},
                    {"totalUser", totalUser},
                    {"createAt", course.CreateAt},
                    {"updateAt", course.UpdateAt}
                };
                courses.Add(courseView);
            };

            var posts = new List<object>();

            foreach(var post in postsDb)
            {
                var user = await _context.Users.FindAsync(post.IdUser);
                var author = new
                {
                    id = user?.Id,
                    avatar = user?.Avatar,
                    userName = user?.UserName
                };

                var postsView = new Dictionary<string, object>
                {
                    {"id", post.Id },
                    {"title", post.Title },
                    {"avatar", post.Avatar },
                    {"slug", post.Slug },
                    {"isPublish", post.IsPublish },
                    {"createAt", post.CreateAt },
                    {"updateAt", post.UpdateAt },
                    {"author", author }
                };

                posts.Add(postsView);
            };

            var home = new
            {
                banners,
                courses,
                posts,
                videos
            };

            return Ok(new { Status = 200, Message = "Success", Data = home });
        }

        [HttpGet("/search")]
        public async Task<IActionResult> SearchHome(string key)
        {
            if (string.IsNullOrEmpty(key))
                return NotFound(new { Status = 404, Message = "Không tìm thấy nội dung" });

            var courses = await _context.Courses.Where(c => c.Title.Contains(key)).ToListAsync();
            var posts = await _context.Posts.Where(p => p.Title.Contains(key)).ToListAsync();
            var videos = await _context.Videos.Where(v => v.Title.Contains(key)).ToListAsync();

            return Ok(new { Status = 200, Message = "Success", Data = new { courses, posts, videos } });
        }
        
        [HttpGet("/badrequest")]        
        public IActionResult BadRequestResult()
        {
            return BadRequest(new { Status = 400, Message = "Yêu cầu không thể thực hiện" });
        }
    }
}
