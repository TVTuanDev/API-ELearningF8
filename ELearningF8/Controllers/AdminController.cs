using ELearningF8.Attributes;
using ELearningF8.Data;
using ELearningF8.Models;
using ELearningF8.Utilities;
using ELearningF8.ViewModel;
using ELearningF8.ViewModel.Course;
using ELearningF8.ViewModel.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace ELearningF8.Controllers
{
    [Route("api/[controller]")]
    [JwtAuthorize]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly MailHandleController _mailHandle;
        private readonly TokenHandle _tokenHandle;

        public AdminController
            (
            AppDbContext context,
            MailHandleController mailHandle,
            TokenHandle tokenHandle
            )
        {
            _context = context;
            _mailHandle = mailHandle;
            _tokenHandle = tokenHandle;
        }

        [HttpPost("/admin/login")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginAdmin(LoginVM model)
        {
            //if (ModelState.IsValid)
            //    return BadRequest(new { Status = 400, Message = "Thông tin nhập vào không hợp lệ" });

            if (await _mailHandle.GetUserByEmail(model.Email) is null)
                return BadRequest(new { Status = 400, Message = "Email chưa được đăng ký" });

            var user = await _mailHandle.GetUserByEmail(model.Email);
            if (user == null)
                return NotFound(new { Status = 404, Message = "Không tìm thấy tài khoản" });

            if (!PasswordManager.VerifyPassword(model.Password, user.HasPassword!))
                return BadRequest(new { Status = 400, Message = "Mật khẩu không đúng" });

            if (!VeryAuthor(user, RoleName.Administrator))
                return new ObjectResult(new { Status = 403, Message = "Bạn không có quyền" })
                {
                    StatusCode = StatusCodes.Status403Forbidden
                };

            var token = new TokenVM
            {
                AccessToken = _tokenHandle.AccessToken(user),
                RefreshToken = _tokenHandle.RefreshToken()
            };

            // Lưu refresh token vào db
            var refreshTokenDb = new RefreshToken
            {
                IdUser = user.Id,
                AccessId = _tokenHandle.GetJti(token.AccessToken),
                Token = token.RefreshToken,
                ExpiredAt = ExpriedToken.Refresh
            };
            _context.Add(refreshTokenDb);
            await _context.SaveChangesAsync();

            //var mailVM = new MailVM
            //{
            //    Email = model.Email
            //};

            //await _mailHandle.SendMailLogin(mailVM);

            return Ok(new { Status = 200, Message = "Success", Data = token });
        }

        #region Users
        [HttpGet("/admin/user")]
        public IActionResult GetUserByType(string type, string? q, int page = 1, int limit = 5)
        {
            var users = _context.Users.Where(u => u.Type == type).ToList();

            var totalUser = users.Count();

            if (!string.IsNullOrEmpty(q))
            {
                users = users.Where(u => u.Email.ToLower().Contains(q.ToLower())).ToList();
            }

            users = users.Skip((page - 1) * limit).Take(limit).ToList();

            return Ok(new { Status = 200, Message = "Success", TotalUser = totalUser, Data = users });
        }

        [HttpPost("/admin/user/create")]
        public async Task<IActionResult> CreateUser(UserVM model)
        {
            var user = new User
            {
                UserName = model.UserName,
                Email = model.Email,
                HasPassword = PasswordManager.HashPassword(model.Password),
                Type = model.Type
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return Ok(new { Status = 200, Message = "Success" });
        }

        [HttpDelete("/admin/user/delete/{id}")]
        public async Task<IActionResult> DeleteUserById(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user != null)
                {
                    _context.Users.Remove(user);
                    await _context.SaveChangesAsync();

                    return Ok(new { Status = 200, Message = "Success" });
                }
                return NotFound(new { Status = 404, Message = $"Không tìm thấy user có id = {id}" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Status = 400, Message = ex.Message });
            }
        }

        [HttpDelete("/admin/user/delete")]
        public async Task<IActionResult> DeleteUserByIds(string ids)
        {
            try
            {
                var listId = ids.Split(",");
                if (listId.Count() < 1)
                    return BadRequest(new { Status = 400, Message = "Id truyền vào không hợp lệ" });

                foreach (var item in listId)
                {
                    int id = Convert.ToInt32(item);
                    var user = await _context.Users.FindAsync(id);
                    if (user is null)
                        return NotFound(new { Status = 404, Message = $"Không tìm thấy user có id = {id}" });

                    _context.Users.Remove(user);
                    await _context.SaveChangesAsync();
                }
                return Ok(new { Status = 200, Message = "Success" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Status = 400, Message = ex.Message });
            }
        }
        #endregion

        #region Courses
        [HttpGet("/admin/course")]
        public async Task<IActionResult> GetCourses(string? q, int page = 1, int limit = 5)
        {
            var course = await _context.Courses.ToListAsync();

            var totalCourse = course.Count();

            if (!string.IsNullOrEmpty(q))
            {
                course = course.Where(u => u.Title.ToLower().Contains(q.ToLower())).ToList();
            }

            course = course.Skip((page - 1) * limit).Take(limit).ToList();

            return Ok(new { Status = 200, Message = "Success", TotalUser = totalCourse, Data = course });
        }

        [HttpPost("/admin/course/create")]
        public async Task<IActionResult> CreateCourse(CourseVM model)
        {
            if (string.IsNullOrEmpty(model.Title))
                return BadRequest(new { Status = 400, Message = "Thông tin truyền vào không hợp lệ" });

            var course = new Course
            {
                Title = model.Title,
                Avatar = model.Avatar,
                Descriptions = model.Descriptions,
                Content = model.Content,
                Slug = AppUtilities.GenerateSlug(model.Title),
                TypeCourse = model.TypeCourse,
                Price = model.Price,
                Discount = model.Discount,
                IsComing = model.IsComing,
                IsPublish = model.IsPublish
            };

            await _context.Courses.AddAsync(course);
            await _context.SaveChangesAsync();

            return Ok(new { Status = 200, Message = "Success" });
        }

        [HttpPatch("/admin/course/update")]
        public async Task<IActionResult> UpdateCourse(CourseVM model)
        {
            var course = await _context.Courses.FindAsync(model.Id);
            if (course is null)
                return NotFound(new { Status = 404, Message = "Không tìm thấy khóa học" });

            course.Title = model.Title;
            course.Avatar = model.Avatar;
            course.Descriptions = model.Descriptions;
            course.Content = model.Content;
            course.Slug = AppUtilities.GenerateSlug(model.Title);
            course.TypeCourse = model.TypeCourse;
            course.Price = model.Price;
            course.Discount = model.Discount;
            course.IsComing = model.IsComing;
            course.IsPublish = model.IsPublish;
            course.UpdateAt = DateTime.UtcNow;

            _context.Courses.Update(course);
            await _context.SaveChangesAsync();

            return Ok(new { Status = 200, Message = "Success" });
        }

        [HttpDelete("/admin/course/delete/{id}")]
        public async Task<IActionResult> DeleteCourseById(int id)
        {
            try
            {
                var course = await _context.Courses.FindAsync(id);
                if (course != null)
                {
                    _context.Courses.Remove(course);
                    await _context.SaveChangesAsync();

                    return Ok(new { Status = 200, Message = "Success" });
                }
                return NotFound(new { Status = 404, Message = $"Không tìm thấy course có id = {id}" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Status = 400, Message = ex.Message });
            }
        }

        [HttpDelete("/admin/course/delete")]
        public async Task<IActionResult> DeleteCourseByIds(string ids)
        {
            try
            {
                var listId = ids.Split(",");
                if (listId.Count() < 1)
                    return BadRequest(new { Status = 400, Message = "Id truyền vào không hợp lệ" });

                foreach (var item in listId)
                {
                    int id = Convert.ToInt32(item);
                    var course = await _context.Courses.FindAsync(id);
                    if (course is null)
                        return NotFound(new { Status = 404, Message = $"Không tìm thấy course có id = {id}" });

                    _context.Courses.Remove(course);
                    await _context.SaveChangesAsync();
                }
                return Ok(new { Status = 200, Message = "Success" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Status = 400, Message = ex.Message });
            }
        }
        #endregion

        #region Chapters
        [HttpPost("/admin/chapter/create")]
        public async Task<IActionResult> CreateChapter(ChapterVM model) 
        {
            if (string.IsNullOrEmpty(model.Title) || model.IdCourse < 1)
                return BadRequest(new { Status = 400, Message = "Thông tin truyền vào không hợp lệ" });

            var chapter = new Chapter
            {
                Title = model.Title,
                Sort = model.Sort,
                IdCourse = model.IdCourse
            };

            await _context.Chapters.AddAsync(chapter);
            await _context.SaveChangesAsync();

            return Ok(new { Status = 200, Message = "Success" });
        }

        [HttpPost("/admin/chapter/update")]
        public async Task<IActionResult> UpdateChapter(ChapterVM model)
        {
            var chapter = await _context.Chapters.FindAsync(model.Id);
            if (chapter is null)
                return NotFound(new { Status = 404, Message = "Không tìm thấy chapter" });

            chapter.Title = model.Title;
            chapter.Sort = model.Sort;
            chapter.IdCourse = model.IdCourse;
            chapter.UpdateAt = DateTime.UtcNow;

            _context.Chapters.Update(chapter);
            await _context.SaveChangesAsync();

            return Ok(new { Status = 200, Message = "Success" });
        }

        [HttpDelete("/admin/chapter/delete/{id}")]
        public async Task<IActionResult> DeleteChapterById(int id)
        {
            try
            {
                var chapter = await _context.Chapters.FindAsync(id);
                if (chapter is null)
                    return NotFound(new { Status = 404, Message = $"Không tìm thấy chapter" });

                _context.Chapters.Remove(chapter);
                await _context.SaveChangesAsync();

                return Ok(new { Status = 200, Message = "Success" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Status = 400, Message = ex.Message });
            }
        }
        #endregion

        #region Lessons
        [HttpGet("/admin/lesson/type")]
        public IActionResult GetTypeLesson()
        {
            var typeLesson = _context.TypeLessons.ToList();

            return Ok(new { Status = 200, Message = "Success", Data = typeLesson });
        }

        [HttpPost("/admin/lesson/create")]
        public async Task<IActionResult> CreateLesson(LessonVM model)
        {
            if (string.IsNullOrEmpty(model.Title) || model.IdChapter < 1)
                return BadRequest(new { Status = 400, Message = "Thông tin truyền vào không hợp lệ" });

            var lesson = new Lesson
            {
                Title = model.Title,
                Sort = model.Sort,
                Content = model.Content,
                Link = model.Link,
                Slug = AppUtilities.GenerateSlug(model.Title),
                IdChapter = model.IdChapter,
                IdType = model.IdType
            };

            await _context.Lessons.AddAsync(lesson);
            await _context.SaveChangesAsync();

            return Ok(new { Status = 200, Message = "Success" });
        }
        
        [HttpPost("/admin/lesson/update")]        
        public async Task<IActionResult> UpdateLesson(LessonVM model)
        {
            var lesson = await _context.Lessons.FindAsync(model.Id);
            if (lesson is null)
                return NotFound(new { Status = 404, Message = "Không tìm thấy lesson" });

            lesson.Title = model.Title;
            lesson.Sort = model.Sort;
            lesson.Content = model.Content;
            lesson.Link = model.Link;
            lesson.Slug = AppUtilities.GenerateSlug(model.Title);
            lesson.UpdateAt = DateTime.UtcNow;

            _context.Lessons.Update(lesson);
            await _context.SaveChangesAsync();

            return Ok(new { Status = 200, Message = "Success" });
        }

        [HttpDelete("/admin/lesson/delete/{id}")]
        public async Task<IActionResult> DeleteLessonById(int id)
        {
            try
            {
                var lesson = await _context.Lessons.FindAsync(id);
                if (lesson is null)
                    return NotFound(new { Status = 404, Message = $"Không tìm thấy lesson" });

                _context.Lessons.Remove(lesson);
                await _context.SaveChangesAsync();

                return Ok(new { Status = 200, Message = "Success" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Status = 400, Message = ex.Message });
            }
        }
        #endregion

        #region Roles
        [HttpGet("/role")]
        public IActionResult GetRoles()
        {
            var roles = _context.Roles.Select(r => new
            {
                r.Id,
                r.RoleName,
                r.CreateAt,
                r.UpdateAt,
            });

            return Ok(new { Status = 200, Message = "Success", Data = roles });
        }

        [HttpPost("/role/create")]
        public async Task<IActionResult> CreateRoles(RoleVM model)
        {
            if (string.IsNullOrEmpty(model.RoleName))
                return BadRequest(new { Status = 400, Message = "Thông tin không hợp lệ" });

            var role = new Role
            {
                RoleName = model.RoleName,
            };

            await _context.Roles.AddAsync(role);
            await _context.SaveChangesAsync();

            return Ok(new { Status = 200, Message = "Success" });
        }

        [HttpPatch("/role/update")]
        public async Task<IActionResult> UpdateRole(RoleVM model)
        {
            var role = _context.Roles.Find(model.Id);
            if (role is null)
                return NotFound(new { Status = 404, Message = "Không tìm thấy role" });

            role.RoleName = model.RoleName;
            role.CreateAt = DateTime.UtcNow;

            _context.Roles.Update(role);
            await _context.SaveChangesAsync();

            return Ok(new { Status = 200, Message = "Success" });
        }

        [HttpDelete("/role/delete")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            var role = _context.Roles.Find(id);
            if (role is null)
                return NotFound(new { Status = 404, Message = "Không tìm thấy role" });

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();

            return Ok(new { Status = 200, Message = "Success" });
        }

        [HttpGet("/role/user")]
        public IActionResult GetRolesByUser()
        {
            var users = _context.Users.ToList();
            var roles = new List<object>();
            foreach (var user in users)
            {
                var roleDb = from ur in _context.UserRoles
                             join r in _context.Roles on ur.IdRole equals r.Id
                             where ur.IdUser == user.Id
                             select r.RoleName;
                var role = new
                {
                    user.UserName,
                    role = roleDb,
                };
                roles.Add(role);
            }

            return Ok(new { Status = 200, Message = "Success", Data = roles });
        }
        #endregion

        private bool VeryAuthor(User user, string role)
        {
            var roleNameDb = from r in _context.Roles
                             join ur in _context.UserRoles on r.Id equals ur.IdRole
                             where ur.IdUser == user.Id
                             select r.RoleName;

            if (!roleNameDb.Contains(role))
                return false;

            return true;
        }
    }
}
