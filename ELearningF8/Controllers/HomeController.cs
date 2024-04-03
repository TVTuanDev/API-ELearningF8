using ELearningF8.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            var courses = await _context.Courses.ToListAsync();
            var posts = await _context.Posts.ToListAsync();
            var videos = await _context.Videos.ToListAsync();

            var home = new
            {
                banners,
                courses,
                posts,
                videos
            };

            return Ok(new { Status = 200, Message = "Success", Data = home });
        }
    }
}
