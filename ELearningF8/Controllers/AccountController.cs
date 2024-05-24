using ELearningF8.Attributes;
using ELearningF8.Data;
using ELearningF8.Models;
using ELearningF8.ViewModel;
using ELearningF8.ViewModel.User;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Asn1.X509;
using System.Dynamic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Nodes;

namespace ELearningF8.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly MailHandleController _mailHandle;
        private readonly IConfiguration _conf;
        private readonly IMemoryCache _cache;
        private readonly TokenHandle _tokenHandle;

        public AccountController
            (
            AppDbContext context,
            MailHandleController mailHandle,
            IConfiguration conf,
            IMemoryCache cache,
            TokenHandle tokenHandle
            )
        {
            _context = context;
            _mailHandle = mailHandle;
            _conf = conf;
            _cache = cache;
            _tokenHandle = tokenHandle;
        }

        [HttpPost("/register")]
        public async Task<IActionResult> RegisterAsync(RegisterVM model)
        {
            try
            {
                //returnUrl ??= Url.Content("~/");
                if (!ModelState.IsValid)
                    return BadRequest(new { Status = 400, Message = "Nhập sai thông tin" });

                if (await _mailHandle.GetUserByEmail(model.Email) != null)
                    return BadRequest(new { Status = 400, Message = "Email đã được sử dụng" });

                var verifyEmail = VerifyEmail(model.Email, model.Code);
                if (verifyEmail)
                {
                    var user = new User
                    {
                        UserName = model.UserName,
                        Email = model.Email,
                        HasPassword = PasswordManager.HashPassword(model.Password)
                    };
                    await _context.AddAsync(user);
                    await _context.SaveChangesAsync();

                    return Ok(new { Status = 200, Message = "Success" });
                }

                return BadRequest(new { Status = 400, Message = "Mã code không còn hiệu lực, vui lòng lấy mã mới" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Status = 400, Message = ex.Message });
            }
        }

        [HttpPost("/login")]
        public async Task<IActionResult> LoginAsync(LoginVM model)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(new { Status = 400, Message = "Nhập sai thông tin" });

                var user = await _mailHandle.GetUserByEmail(model.Email);

                if (user != null)
                {
                    if (user.Status == "block")
                        return BadRequest(new { Status = 400, Message = "Tài khoản đang bị khóa" });

                    var checkPassword = PasswordManager.VerifyPassword(model.Password, user.HasPassword!);
                    if (checkPassword)
                    {
                        #region LockedOut
                        //if (user.IsLockedOut)
                        //{
                        //    await _mailHandle.SendCodeMailLogin(user.Email);
                        //}
                        #endregion
                        // Cấp accessToken và refreshToken
                        var token = new TokenVM
                        {
                            AccessToken = _tokenHandle.AccessToken(user),
                            RefreshToken = _tokenHandle.RefreshToken(),
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

                        return Ok(new { Status = 200, Message = "Success", token });
                    }
                    return BadRequest(new { Status = 400, Message = "Tài khoản hoặc mật khẩu không đúng" });
                }
                return BadRequest(new { Status = 400, Message = "Tài khoản hoặc mật khẩu không đúng" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Status = 400, Message = ex.Message });
            }
        }

        [HttpGet("/logout")]
        [JwtAuthorize]
        public async Task<IActionResult> LogOut()
        {
            var tokenContext = HttpContext?.Request.Headers.Authorization.FirstOrDefault()?.Split(" ").Last();

            if (string.IsNullOrEmpty(tokenContext))
                return NotFound(new { Status = 404, Message = "Không tìm thấy access token" });

            int.TryParse(HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier), out int idUser);
            var user = await _context.Users.FindAsync(idUser);
            if (user is null) return BadRequest(new { Status = 400, Message = "Không tìm thấy user trong access token" });

            var blackListDb = _context.BlackLists.FirstOrDefault(bl => bl.AccessToken == tokenContext);

            if (blackListDb is not null)
                return BadRequest(new { Status = 400, Message = "Access token đã tồn tại trong black list" });

            var blackList = new BlackList
            {
                AccessToken = tokenContext
            };

            await _context.BlackLists.AddAsync(blackList);
            await _context.SaveChangesAsync();

            return Ok(new { Status = 200, Message = "Success" });
        }

        [HttpPost("/refresh-token")]
        public async Task<IActionResult> RefreshToken(TokenVM tokenVM)
        {
            if (!string.IsNullOrEmpty(tokenVM.RefreshToken))
            {
                try
                {
                    // 1. Kiểm tra có refresh trong db
                    var refreshDb = _context.RefreshTokens.FirstOrDefault(rt => rt.Token == tokenVM.RefreshToken);
                    if (refreshDb is null)
                        return NotFound(new { Status = 404, Message = "Không tìm thấy refresh token trong database" });

                    // 2. Check refresh hết hạn hay chưa
                    if (refreshDb.ExpiredAt < DateTime.UtcNow)
                        return BadRequest(new { Status = 400, Message = "Refresh token đã hết hạn, yêu cầu đăng nhập lại" });

                    // 3. Check refresh đã được sử dụng hay chưa
                    if (refreshDb.IsUsed == true)
                        return BadRequest(new { Status = 400, Message = "Refresh token đã được sử dụng, yêu cầu đăng nhập lại" });

                    var user = await _context.Users.FindAsync(refreshDb.IdUser);
                    if (user is null)
                        return BadRequest(new { Status = 400, Message = "Refresh token không chứa id user" });

                    var token = new TokenVM
                    {
                        AccessToken = _tokenHandle.AccessToken(user),
                        RefreshToken = _tokenHandle.RefreshToken()
                    };

                    // Cập nhật refresh đã được sử dụng
                    refreshDb.IsUsed = true;
                    _context.RefreshTokens.Update(refreshDb);

                    // Lưu refresh token mới
                    var newRefresh = new RefreshToken
                    {
                        IdUser = user.Id,
                        AccessId = _tokenHandle.GetJti(token.AccessToken),
                        Token = token.RefreshToken,
                        ExpiredAt = ExpriedToken.Refresh
                    };

                    _context.RefreshTokens.Add(newRefresh);
                    await _context.SaveChangesAsync();

                    return Ok(new { Status = 200, Message = "Success", token });
                }
                catch (Exception ex)
                {
                    return BadRequest(new { Status = 400, Message = ex.Message });
                }
            }
            return NotFound(new { Status = 400, Message = "Không tìm thấy refresh token" });
        }

        [NonAction] // Dành cho function ko phải action api 
        private bool VerifyEmail(string email, string code)
        {
            // Check email và code
            var getCache = _cache.Get<string>(email);
            if (getCache == code) return true;
            return false;
        }

        [HttpGet("/user")]
        [JwtAuthorize(RoleName.Administrator)]
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                var users = await _context.Users.ToListAsync();
                return Ok(new { Status = 200, Message = "Success", Data = users });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Status = 400, Message = ex?.Message });
            }
        }

        [HttpGet("/user/{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            try
            {
                if (id > 0)
                {
                    var user = await _context.Users.Where(u => u.Id == id)
                        .Select(u => new
                        {
                            u.Id,
                            u.UserName,
                            u.Email,
                            u.Phone,
                            u.Avatar,
                            u.BgAvatar,
                            u.Status,
                            u.Providers,
                            u.TwoFactorEnabled,
                            u.CreateAt,
                            u.UpdateAt
                        }).FirstOrDefaultAsync();
                    if (user != null) return Ok(new { Status = 200, Message = "Success", Data = user });
                }
                return NotFound(new { Status = 400, Message = "Không tìm thấy user" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Status = 400, Message = ex?.Message });
            }
        }

        [HttpGet("/user/profile")]
        [JwtAuthorize]
        public async Task<IActionResult> GetProfileUser()
        {
            try
            {
                var nameIdentity = HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
                int.TryParse(nameIdentity, out int idUser);

                var courses = await (from uc in _context.UserCourses
                                     join c in _context.Courses on uc.IdCourse equals c.Id
                                     where uc.IdUser == idUser
                                     select c)
                                     .Include(c => c.Chapters)
                                        .ThenInclude(ch => ch.Lessons)
                                     .ToListAsync();

                var posts = await _context.Posts.Where(p => p.IdUser == idUser).ToListAsync();

                var user = await _context.Users.Where(u => u.Id == idUser).FirstOrDefaultAsync();

                if (user != null) return Ok(new { Status = 200, Message = "Success", Data = user });
                return NotFound(new { Status = 400, Message = "Không tìm thấy user" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Status = 400, Message = ex.Message });
            }
        }

        [HttpPatch("/user/update/profile")]
        [JwtAuthorize]
        public async Task<IActionResult> UpdateUser([FromBody] ExpandoObject data)
        {
            try
            {
                var dynamicData = (IDictionary<string, object>)data!;

                int.TryParse(dynamicData["id"].ToString(), out int idUser);

                var user = await _context.Users.FindAsync(idUser);
                if (user is null)
                    return NotFound(new { Status = 404, Message = "Không tìm thấy user" });

                foreach (var model in dynamicData)
                {
                    string key = char.ToUpper(model.Key[0]) + model.Key.Substring(1);
                    var value = model.Value.ToString();

                    if (key == "Id")
                        continue;
                    if (key == "UserName" && string.IsNullOrEmpty(value)) 
                        return BadRequest(new { Status = 400, Message = "UserName không được null" });
                    if (key == "Email" && !_mailHandle.CheckRegexMail(value!))
                        return BadRequest(new { Status = 400, Message = "Không đúng định dạng email" });
                    if (key == "Password")
                    {
                        if(value?.Count() < 8)
                            return BadRequest(new { Status = 400, Message = "Mật khẩu phải dài hơn 8 ký tự" });
                        key = "HasPassword";
                        value = PasswordManager.HashPassword(value!);
                    }

                    PropertyInfo property = typeof(User).GetProperty(key)!;
                    if (property != null && property.CanWrite)
                    {
                        // Kiểm tra xem thuộc tính có thể ghi được không
                        property.SetValue(user, Convert.ChangeType(value, property.PropertyType));
                    }
                }

                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                return Ok(new { Status = 200, Message = "Success" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Status = 400, Message = ex.Message });
            }
        }

        [HttpPatch("/user/update/bg-avatar")]
        [JwtAuthorize]
        public async Task<IActionResult> UpdateUserBgAvatar(UserVM model)
        {
            try
            {
                var user = await _context.Users.FindAsync(model.Id);
                if (user is null) return NotFound(new { Status = 404, Message = "Không tìm thấy user" });

                user.BgAvatar = model.BgAvatar ?? user.BgAvatar;

                _context.Update(user);
                await _context.SaveChangesAsync();
                return Ok(new { Status = 200, Message = "Success" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Status = 400, Message = ex.Message });
            }
        }

        [HttpGet("/external-login")]
        public IActionResult ExternalLogin()
        {
            var properties = new AuthenticationProperties { RedirectUri = Url.Action("ExternalLoginCallback", "Account") };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);

            //await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme,
            //    new AuthenticationProperties
            //    {
            //        RedirectUri = Url.Action("ExternalLoginCallback")
            //    });
            //var redirectUrl = Url.Action(nameof(HandleExternalLogin), "Account", new { returnUrl });
            //var properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);
            //return Challenge(properties, "Google");
        }

        [HttpGet("/external-login-callback")]
        public IActionResult ExternalLoginCallback()
        {
            return Content("Xác thực thành công!");
            //try
            //{
            //    var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            //    if (result.Succeeded)
            //    {
            //        var provider = result.Ticket?.AuthenticationScheme ?? "";
            //        var id = result.Principal?.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
            //        var name = result.Principal?.FindFirstValue(ClaimTypes.Name) ?? "";
            //        var email = result.Principal?.FindFirstValue(ClaimTypes.Email) ?? "";

            //        var user = await _mailHandle.GetUserByEmail(email, provider);

            //        if (user == null)
            //        {
            //            user = new User
            //            {
            //                UserName = name,
            //                Email = email,
            //                Providers = provider
            //            };
            //            _context.Users.Add(user);
            //            await _context.SaveChangesAsync();

            //            var userLogin = new UserLogin
            //            {
            //                IdUser = user.Id,
            //                LoginProvider = provider,
            //                ProviderKey = id,
            //                ProviderDisplayName = provider
            //            };
            //            _context.UserLogins.Add(userLogin);
            //            await _context.SaveChangesAsync();
            //        }

            //        var token = new TokenVM
            //        {
            //            AccessToken = GenerateToken(user, _expriedToken.Access),
            //            RefreshToken = GenerateToken(user, _expriedToken.Refresh),
            //        };

            //        // Lưu refresh token vào db
            //        var refreshTokenDb = new RefreshToken
            //        {
            //            IdUser = user.Id,
            //            JwtId = GetJti(token.RefreshToken),
            //            Token = token.RefreshToken,
            //            ExpiredAt = _expriedToken.Refresh
            //        };
            //        _context.RefreshTokens.Add(refreshTokenDb);
            //        await _context.SaveChangesAsync();

            //        return Ok(new { Status = 200, Message = "Success", token });
            //        //return RedirectPermanent(returnUrl);
            //    }
            //    else
            //    {
            //        return BadRequest(new { Status = 400, Message = result.Failure?.Message });
            //    }

            //}
            //catch (Exception ex)
            //{
            //    return BadRequest(new { Status = 400, Message = ex.Message });
            //}

            //var info = await _signInManager.GetExternalLoginInfoAsync();
            //if (info == null)
            //{
            //    // Handle error
            //    return RedirectToAction(nameof(ExternalLogin));
            //}

            //// Retrieve user information from the external provider
            //var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            //var name = info.Principal.FindFirstValue(ClaimTypes.Name);
            //// Other information as needed

            //// Logic to handle user information
            //// Redirect user to the appropriate page
            //return Redirect(returnUrl);
            //var claims = result.Principal?.Claims.Select(claim => new
            //{
            //    claim.Issuer,
            //    claim.OriginalIssuer,
            //    claim.Type,
            //    claim.Value,
            //    email,
            //    name,
            //    id
            //});
        }

        [HttpGet("/user/random/{count}")]
        public async Task<IActionResult> RandomUsers(int count)
        {
            string[] firstName = { "Nguyễn", "Trần", "Lê", "Đào", "Phạm", "Hoàng", "Huỳnh", "Phan", "Vũ", "Võ", "Đặng" };
            string[] lastName = { "Nam", "Quốc", "Ánh", "Hải", "Thành", "Tuyết", "Anh", "Ngọc", "Tùng", "Đạt", "Công",
                "Lộc", "Loan", "My", "Phương", "Dũng", "Bích", "Ly", "Khải", "Hưng", "Hà", "Mạnh", "Linh" };

            Random random = new Random();

            for (int i = 0; i < count; i++)
            {
                int randomFirstNameIndex = random.Next(0, firstName.Length);
                int randomLastNameIndex = random.Next(0, lastName.Length);
                string firstNameRandom = firstName[randomFirstNameIndex];
                string lastNameRandom = lastName[randomLastNameIndex];
                string name = firstNameRandom + " " + lastNameRandom;
                string email = ConvertModel.RemoveDiacriticsAndSpaces(name) + "@gmail.com";

                var user = new User
                {
                    UserName = name,
                    Email = email.ToLower(),
                    HasPassword = PasswordManager.HashPassword("12345678")
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }

            return Ok(new { Status = 200, Message = "Success"});
        }
    }
}
