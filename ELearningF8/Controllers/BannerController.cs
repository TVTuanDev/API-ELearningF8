using ELearningF8.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ELearningF8.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BannerController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BannerController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("/banner")]
        public async Task<IActionResult> GetBanner()
        {
            var banner = await _context.Banners.ToListAsync();

            if (banner == null) return BadRequest(new { Status = 400, Message = "Không tìm thấy banner" });

            return Ok(new { Status = 200, Message = "Ok", Data = banner });
        }
    }
}
