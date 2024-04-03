using ELearningF8.Data;
using ELearningF8.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ELearningF8.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PostController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("/post")]
        public async Task<IActionResult> GetPosts()
        {
            var posts = await _context.Posts.ToListAsync();

            return Ok(new { Status = 200, Message = "Success", Data = posts });
        }
    }
}
