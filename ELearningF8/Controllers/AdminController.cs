using ELearningF8.Attributes;
using ELearningF8.Data;
using ELearningF8.Models;
using ELearningF8.ViewModel;
using ELearningF8.ViewModel.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace ELearningF8.Controllers
{
    [Route("api/[controller]")]
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
        public async Task<IActionResult> LoginAdmin(LoginVM model)
        {
            //if (ModelState.IsValid)
            //    return BadRequest(new { Status = 400, Message = "Thông tin nhập vào không hợp lệ" });

            if (await _mailHandle.GetUserByEmail(model.Email) is null)
                return BadRequest(new { Status = 400, Message = "Email chưa được đăng ký" });

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
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
                AccessToken = _tokenHandle.AccessToken(user, ExpriedToken.Access),
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

        [HttpGet("/admin/users")]
        //[JwtAuthorize(RoleName = )]
        public IActionResult GetUserByType(string type)
        {
            var users = _context.Users.Where(u => u.Type == type).ToList();

            return Ok(new { Status = 200, Message = "Success", Data = users });
        }

        [HttpPost("/admin/create-user")]
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
            if(string.IsNullOrEmpty(model.RoleName))
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
            if(role is null)
                return NotFound(new { Status = 404, Message = "Không tìm thấy role" });

            role.RoleName = model.RoleName;
            role.CreateAt = DateTime.UtcNow;

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

        private bool VeryAuthor (User user, string role)
        {
            var roleNameDb = from r in _context.Roles
                             join ur in _context.UserRoles on r.Id equals ur.IdRole
                             where ur.IdUser == user.Id
                             select r.RoleName;

            if (!roleNameDb.Contains(RoleName.Administrator))
                return false;

            return true;
        }
    }
}
