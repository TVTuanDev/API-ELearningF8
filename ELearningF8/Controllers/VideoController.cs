using ELearningF8.Data;
using ELearningF8.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ELearningF8.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public VideoController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("/video")]
        public async Task<IActionResult> GetVideos()
        {
            var videos = await _context.Videos.ToListAsync();

            return Ok(new { Status = 200, Message = "Success", Data = videos });
        }
    }
}
