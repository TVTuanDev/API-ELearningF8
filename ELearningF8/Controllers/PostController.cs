using ELearningF8.Attributes;
using ELearningF8.Data;
using ELearningF8.Utilities;
using ELearningF8.ViewModel;
using ELearningF8.ViewModel.Post;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.Extensions.Hosting;
using System.Security.Claims;

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
            var postsDb = await _context.Posts.OrderBy(p => p.CreateAt).ToListAsync();

            var posts = new List<object>();

            foreach (var post in postsDb)
            {
                var user = await _context.Users.FindAsync(post.IdUser);
                var author = new
                {
                    id = user?.Id,
                    avatar = user?.Avatar,
                    userName = user?.UserName
                };

                var tags = await (from pt in _context.PostTags
                            join t in _context.Tags on pt.IdTag equals t.Id
                            where pt.IdPost == post.Id
                            select new
                            {
                                t.Id,
                                t.TagName
                            }).ToListAsync();

                var postsView = new Dictionary<string, object>
                {
                    {"id", post.Id },
                    {"title", post.Title },
                    {"avatar", post.Avatar },
                    {"descriptions", post.Descriptions ?? "" },
                    {"content", post.Content ?? "" },
                    {"slug", post.Slug ?? "" },
                    {"isPublish", post.IsPublish },
                    {"createAt", post.CreateAt },
                    {"updateAt", post.UpdateAt },
                    {"author", author },
                    {"tags", tags }
                };

                posts.Add(postsView);
            };

            return Ok(new { Status = 200, Message = "Success", Data = posts });
        }

        [HttpGet("/post/{slug}")]
        public async Task<IActionResult> GetPostById(string slug)
        {
            if (string.IsNullOrEmpty(slug)) return BadRequest(new { Status = 400, Message = "Slug truyền vào không hợp lệ" });

            var postDb = await _context.Posts.FirstOrDefaultAsync(p => p.Slug == slug);
            if (postDb == null) return BadRequest(new { Status = 400, Message = "Không tìm thấy bài viết" });

            var user = await _context.Users.FindAsync(postDb.IdUser);
            if (user == null) return BadRequest(new { Status = 400, Message = "Không tìm thấy author" });

            var author = new
            {
                id = user.Id,
                avatar = user.Avatar,
                userName = user.UserName
            };

            var tags = await (from pt in _context.PostTags
                              join t in _context.Tags on pt.IdTag equals t.Id
                              where pt.IdPost == postDb.Id
                              select new
                              {
                                  t.Id,
                                  t.TagName
                              }).ToListAsync();

            var post = new Dictionary<string, object>
            {
                {"id", postDb.Id },
                {"title", postDb.Title },
                {"avatar", postDb.Avatar },
                {"descriptions", postDb.Descriptions ?? "" },
                {"content", postDb.Content ?? "" },
                {"slug", postDb.Slug ?? "" },
                {"isPublish", postDb.IsPublish },
                {"createAt", postDb.CreateAt },
                {"updateAt", postDb.UpdateAt },
                {"author", author },
                {"tags", tags }
            };

            return Ok(new { Status = 200, Message = "Success", Data = post });
        }

        [HttpGet("/blog")]
        public async Task<IActionResult> PaggingPost(int page = 1)
        {
            int totalPost = await _context.Posts.CountAsync();
            int pageSize = 10;
            int checkPage = (int)Math.Ceiling(((double)totalPost / pageSize));
            if (page < 1 || checkPage < page)
                return NotFound(new { Status = 404, Message = "Không có nội dung" });

            var postInPage = await _context.Posts.OrderByDescending(p => p.CreateAt)
                .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            var posts = new List<object>();

            foreach (var post in postInPage)
            {
                var user = await _context.Users.FindAsync(post.IdUser);
                var author = new
                {
                    id = user?.Id,
                    avatar = user?.Avatar,
                    userName = user?.UserName
                };

                var tags = await (from pt in _context.PostTags
                                  join t in _context.Tags on pt.IdTag equals t.Id
                                  where pt.IdPost == post.Id
                                  select new
                                  {
                                      t.Id,
                                      t.TagName
                                  }).ToListAsync();

                var postsView = new Dictionary<string, object>
                {
                    {"id", post.Id },
                    {"title", post.Title },
                    {"avatar", post.Avatar },
                    {"descriptions", post.Descriptions },
                    {"slug", post.Slug },
                    {"isPublish", post.IsPublish },
                    {"createAt", post.CreateAt },
                    {"updateAt", post.UpdateAt },
                    {"author", author },
                    {"tags", tags }
                };
                posts.Add(postsView);
            }

            return Ok(new { Status = 200, Message = "Success", TotalPost = totalPost,  Data = posts });
        }

        [HttpGet("/convert-slug")]
        public IActionResult ConvertSlug(string title)
        {
            if (string.IsNullOrEmpty(title)) return BadRequest();

            var slug = AppUtilities.GenerateSlug(title) + ".html";

            return Ok(new { slug });
        }

        [HttpPost("/post/new")]
        [JwtAuthorize]
        public async Task<IActionResult> CreatePost(PostVM model)
        {
            if (!ModelState.IsValid) return BadRequest(new { Status = 400, Message = "Nhập sai thông tin" });

            try
            {
                int.TryParse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier), out var idUser);
                if (idUser == 0) return NotFound(new { Status = 404, Message = "Không tìm thấy user" });

                var post = new Post
                {
                    Title = model.Title,
                    Avatar = model.Avatar,
                    Descriptions = model.Descriptions,
                    Content = model.Content,
                    Slug = AppUtilities.GenerateSlug(model.Title) + ".html",
                    IsPublish = model.IsPublish,
                    IdUser = idUser
                };

                _context.Posts.Add(post);
                await _context.SaveChangesAsync();

                // Xử lý tag
                var tags = model.Tag?.Replace(" ", "").Split(",");
                if (tags?.Length > 0)
                {
                    foreach (var tag in tags)
                    {
                        var tagDb = _context.Tags.FirstOrDefault(t => t.TagName == tag);
                        // Nếu không có tag trong db thì thêm mới
                        if (tagDb is null)
                        {
                            tagDb = new Tag
                            {
                                TagName = tag
                            };
                            _context.Tags.Add(tagDb);
                            await _context.SaveChangesAsync();
                        }

                        var postTags = new PostTag
                        {
                            IdPost = post.Id,
                            IdTag = tagDb.Id
                        };

                        _context.PostTags.Add(postTags);
                        await _context.SaveChangesAsync();
                    }
                }

                return Ok(new { Status = 200, Message = "Success" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Status = 200, Message = ex.Message });
            }
        }

        [HttpPatch("/post/update")]
        [JwtAuthorize]
        public async Task<IActionResult> UpdatePost(PostVM model)
        {
            if (!ModelState.IsValid) return BadRequest(new { Status = 400, Message = "Nhập sai thông tin" });

            var post = await _context.Posts.FindAsync(model.Id);
            if (post == null) return NotFound(new { Status = 404, Message = "Không tìm thấy bài viết" });

            int.TryParse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier), out var idUser);
            if (post.IdUser != idUser)
                return BadRequest(new { Status = 400, Message = "Bạn không phải là tác giả của bài viết này" });

            post.Title = model.Title;
            post.Avatar = model.Avatar ?? post.Avatar;
            post.Descriptions = model.Descriptions ?? post.Descriptions;
            post.Content = model.Content ?? post.Content;
            post.Slug = AppUtilities.GenerateSlug(model.Title) + ".html";
            post.IsPublish = model.IsPublish;
            post.UpdateAt = DateTime.UtcNow;

            // Xử lý tag
            // Xóa tag trong post
            var postTag = await _context.PostTags.Where(pt => pt.IdPost == post.Id).ToListAsync();
            _context.PostTags.RemoveRange(postTag);
            await _context.SaveChangesAsync();

            var tags = model.Tag?.Replace(" ", "").Split(",");
            if (tags?.Length > 0)
            {
                foreach (var tag in tags)
                {
                    var tagDb = _context.Tags.FirstOrDefault(t => t.TagName == tag);
                    // Nếu không có tag trong db thì thêm mới
                    if (tagDb is null)
                    {
                        tagDb = new Tag
                        {
                            TagName = tag
                        };
                        _context.Tags.Add(tagDb);
                        await _context.SaveChangesAsync();
                    }

                    var postTags = new PostTag
                    {
                        IdPost = post.Id,
                        IdTag = tagDb.Id
                    };

                    _context.PostTags.Add(postTags);
                    await _context.SaveChangesAsync();
                }
            }

            _context.Posts.Update(post);
            await _context.SaveChangesAsync();

            return Ok(new { Status = 200, Message = "Success" });
        }

        [HttpDelete("/post/delete")]
        [JwtAuthorize]
        public async Task<IActionResult> DeletePost(int id)
        {
            if (id == 0) BadRequest(new { Status = 400, Message = "Nhập sai thông tin" });

            var post = await _context.Posts.FindAsync(id);
            if (post == null) return NotFound(new { Status = 404, Message = "Không tìm thấy bài viết" });

            int.TryParse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier), out var idUser);
            if (post.IdUser != idUser)
                return BadRequest(new { Status = 400, Message = "Bạn không phải là tác giả của bài viết này" });

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            return Ok(new { Status = 200, Message = "Success" });
        }
    }
}
