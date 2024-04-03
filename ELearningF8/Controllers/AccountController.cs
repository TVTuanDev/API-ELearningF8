using ELearningF8.Attributes;
using ELearningF8.Data;
using ELearningF8.Models;
using ELearningF8.ViewModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ELearningF8.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly PasswordManager _passwordManager;
        private readonly MailHandleController _mailHandle;
        private readonly IConfiguration _conf;
        private readonly IMemoryCache _cache;
        private readonly ExpriedToken _expriedToken;
        private readonly MediaController _media;

        public AccountController
            (
            AppDbContext context,
            PasswordManager passwordManager,
            MailHandleController mailHandle,
            IConfiguration conf,
            IMemoryCache cache,
            ExpriedToken expriedToken,
            MediaController media
            )
        {
            _context = context;
            _passwordManager = passwordManager;
            _mailHandle = mailHandle;
            _conf = conf;
            _cache = cache;
            _expriedToken = expriedToken;
            _media = media;
        }

        [HttpPost("/register")]
        public async Task<IActionResult> RegisterAsync(RegisterVM model)
        {
            try
            {
                //returnUrl ??= Url.Content("~/");
                if (!ModelState.IsValid) return BadRequest(new { Status = 400, Message = "Nhập sai thông tin" });
                if (await _mailHandle.GetUserByEmail(model.Email) != null) return BadRequest(new { Status = 400, Message = "Email đã được sử dụng" });

                var verifyEmail = VerifyEmail(model.Email, model.Code);
                if (verifyEmail)
                {
                    var user = new User
                    {
                        UserName = model.UserName,
                        Email = model.Email,
                        HasPassword = _passwordManager.HashPassword(model.Password)
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
                    if (user.Status == "block") return BadRequest(new { Status = 400, Message = "Tài khoản đang bị khóa" });
                    var checkPassword = _passwordManager.VerifyPassword(model.Password, user.HasPassword);
                    if (checkPassword)
                    {
                        // Kiểm tra có xác thực 2 yếu tố
                        //if (user.IsLockedOut)
                        //{
                        //    await _mailHandle.SendCodeMailLogin(user.Email);
                        //}
                        // Cấp accessToken và refreshToken
                        var token = new TokenVM
                        {
                            AccessToken = GenerateToken(user, _expriedToken.Access),
                            RefreshToken = GenerateToken(user, _expriedToken.Refresh),
                        };

                        // Lưu refresh token vào db
                        var refreshTokenDb = new RefreshToken
                        {
                            IdUser = user.Id,
                            JwtId = GetJti(token.RefreshToken),
                            Token = token.RefreshToken,
                            ExpiredAt = _expriedToken.Refresh
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

        private string GenerateToken(User user, DateTime exprires)
        {
            // Tạo đối tượng để tạo mới jwt hoặc xác minh các jwt
            var tokenHandler = new JwtSecurityTokenHandler();
            // Lấy khóa bí mật trong file appsetting.json
            // Khóa này được sử dụng để ký và xác minh jwt
            var secretKey = Encoding.UTF8.GetBytes(_conf["Jwt:SecretKey"] ?? "");
            // Tạo token chứa thông tin jwt
            var accessTokenDesc = new SecurityTokenDescriptor
            {
                // Chứa thông tin về người dùng được định danh trong JWT
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim("id", user.Id.ToString()),
                    // Tạo id cho token
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(ClaimTypes.Name, user.UserName)
                }),
                // Thời gian hết hạn của jwt
                Expires = exprires,
                // Loại xác thực để ký jwt
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256Signature)
            };
            // Tạo token dựa trên thông tin của tokenDescriptior
            var token = tokenHandler.CreateToken(accessTokenDesc);
            // Chuyển đổi jwt thành string
            var tokenString = tokenHandler.WriteToken(token);

            return tokenString;
        }

        private string GetJti(string refreshToken)
        {
            var jwtToken = new JwtSecurityToken(refreshToken);
            var jti = jwtToken.Payload[JwtRegisteredClaimNames.Jti]?.ToString();
            if (!string.IsNullOrEmpty(jti)) return jti;
            return "";
        }

        [HttpPost("/refresh-token")]
        public async Task<IActionResult> RefreshToken(RefreshTokenVM refreshToken)
        {
            if (!string.IsNullOrEmpty(refreshToken.RefreshToken))
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var secretKey = Encoding.UTF8.GetBytes(_conf["Jwt:SecretKey"] ?? "");
                try
                {
                    tokenHandler.ValidateToken(refreshToken.RefreshToken, new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(secretKey),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        //ValidIssuer = "yourdomain.com",
                        //ValidAudience = "yourdomain.com",
                        ValidateLifetime = false,
                        ClockSkew = TimeSpan.Zero
                    }, out SecurityToken validatedToken);
                    var jwtToken = (JwtSecurityToken)validatedToken;

                    // 1.Check refresh token có đúng kiểu ký
                    if (jwtToken is JwtSecurityToken)
                    {
                        var result = jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);
                        if (!result) return BadRequest(new { Status = 400, Message = "Sai định dạng token" });
                    }

                    // 2.Check refresh token đã hết hạn chưa
                    if (jwtToken.ValidTo < DateTime.UtcNow)
                    {
                        // Refresh token hết hạn
                        return Unauthorized(new { Status = 401, Message = "Refresh token đã hết hạn, yêu cầu đăng nhập lại" });
                    }

                    string jtiValue = jwtToken.Payload[JwtRegisteredClaimNames.Jti]?.ToString();

                    // 3.Check refresh token có trong db hay không
                    var refreshTokenDb = _context.RefreshTokens.FirstOrDefault(rt => rt.JwtId == jtiValue);
                    if (refreshTokenDb == null) return BadRequest(new { Status = 400, Message = "Không tìm thấy refresh token trong sql" });

                    // 4.Check refresh token đã được sử dụng hay chưa
                    if (refreshTokenDb?.IsUsed == true) return BadRequest(new { Status = 400, Message = "Refresh token đã được sử dụng" });

                    //var claims = jwtToken.Claims;
                    //var nameIdentifierClaim = claims.FirstOrDefault(c => c.Type == "IdUser");
                    //int.TryParse(nameIdentifierClaim?.Value, out int idUser);
                    var user = await _context.Users.FindAsync(refreshTokenDb?.IdUser);
                    if (user != null)
                    {
                        // Tạo mới access token và refresh token
                        var tokenModel = new TokenVM
                        {
                            AccessToken = GenerateToken(user, _expriedToken.Access),
                            RefreshToken = refreshToken.RefreshToken
                        };
                        // Cập nhật db refresh cũ đã được sử dụng
                        refreshTokenDb.IsUsed = true;
                        await _context.SaveChangesAsync();

                        return Ok(new { Status = 200, Message = "Success", tokenModel });
                    }
                    return BadRequest(new { Status = 400, Message = "Không tìm thấy id user trong refresh token" });
                }
                catch (Exception ex)
                {
                    return BadRequest(new { Status = 400, Message = ex.Message });
                }
            }
            return BadRequest(new { Status = 400, Message = "Không tìm thấy refresh token" });
        }

        [NonAction] // Dành cho function ko phải action api 
        private bool VerifyEmail(string email, string code)
        {
            // Check email và code
            var getCache = _cache.Get<string>(email);
            if (getCache == code) return true;
            return false;
        }

        //[HttpPost]
        //[JwtAuthorize]
        //public async Task<IActionResult> ForgotPassword(ForgotPasswordVM model)
        //{
        //    if (!ModelState.IsValid) return BadRequest(new { Status = 400, Message = "Nhập sai thông tin" });
        //    if (!VerifyEmail(model.Email, model.Code)) return BadRequest(new { Status = 400, Message = "Code không chính xác" });
            
        //    var user = await _mailHandle.GetUserByEmail(model.Email);
        //    if (user == null) return BadRequest(new { Status = 400, Message = $"Không tìm thấy user có email = {model.Email}" });

        //}

        [HttpGet("/user")]
        [JwtAuthorize]
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                var users = await _context.Users
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
                    }).ToListAsync();
                return Ok(new { Status = 200, Message = "Success", Data = users });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Status = 400, Message = ex?.Message });
            }
        }

        [HttpGet("/user/{id}")]
        [JwtAuthorize]
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
                return BadRequest(new { Status = 400, Message = "Không tìm thấy user" });
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
                int.TryParse(HttpContext.Items["IdUser"]?.ToString(), out int idUser);
                //var httpContext = _contextAccessor.HttpContext;
                //var nameIdentity = httpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
                //int.TryParse(nameIdentity, out int idUser);
                var user = await _context.Users.Where(u => u.Id == idUser)
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
                return BadRequest(new { Status = 400, Message = "Không tìm thấy user" });
            }
            catch (Exception ex)
            {
                //return JsonResult
                //return new ObjectResult(new { Status = 400, Message = ex.Message })
                //{
                //    StatusCode = 400
                //};
                return BadRequest(new { Status = 400, Message = ex.Message });
            }
        }

        [HttpDelete("/user/delete/{id}")]
        [JwtAuthorize]
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
                return BadRequest(new { Status = 400, Message = $"Không tìm thấy user có id = {id}" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Status = 400, Message = ex.Message });
            }
        }

        [HttpDelete("/user/delete")]
        [JwtAuthorize]
        public async Task<IActionResult> DeleteUsersByIds([FromBody] IdRequestVM idRequest)
        {
            try
            {
                if (idRequest.Ids.Count() > 0)
                {
                    foreach (var id in idRequest.Ids)
                    {
                        var user = await _context.Users.FindAsync(id);
                        if (user != null)
                        {
                            _context.Users.Remove(user);
                            await _context.SaveChangesAsync();
                            continue;
                        }

                        return BadRequest(new { Status = 400, Message = $"Không tìm thấy user có id = {id}" });
                    }
                    return Ok(new { Status = 200, Message = "Success" });
                }

                return BadRequest(new { Status = 400, Message = "Id truyền vào không hợp lệ" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Status = 400, Message = ex.Message });
            }
        }

        [HttpPatch("/user/update")]
        [JwtAuthorize]
        public async Task<IActionResult> UpdateUser(UserVM model)
        {
            //ModelState.Remove(model.Password);
            //if (!ModelState.IsValid) return BadRequest(new { Status = 400, Message = "Nhập sai thông tin" });
            //var userId = User.FindFirst("id")?.Value;
            //return Ok(userId);
            try
            {
                var user = await _context.Users.FindAsync(model.Id);
                if (user != null)
                {
                    // user dky = email và mk != null
                    if (user.Providers == "email" && model.Password == null)
                    {
                        return BadRequest(new { Status = 400, Message = "Mật khẩu không được để trống" });
                    }
                    int countPass = 8;
                    if (user.Providers == "email" && model.Password?.Count() < countPass)
                    {
                        return BadRequest(new { Status = 400, Message = $"Mật khẩu phải lớn hơn {countPass} ký tự" });
                    }

                    user.UserName = model.UserName;
                    user.HasPassword = user.Providers == "email" ? _passwordManager.HashPassword(model.Password) : user.HasPassword;
                    user.Bio = model.Bio;
                    user.Avatar = await _media.SaveImageAsync(model.Avatar) ?? user.Avatar;
                    user.BgAvatar = await _media.SaveImageAsync(model.BgAvatar) ?? user.BgAvatar;
                    user.Status = model.Status;
                    user.TwoFactorEnabled = model.TwoFactorEnabled;
                    user.UpdateAt = DateTime.UtcNow;
                    
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                    return Ok(new { Status = 200, Message = "Success" });
                }
                return BadRequest(new { Status = 400, Message = "Không tìm thấy user" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Status = 400, Message = ex.Message });
            }
        }

        //[NonAction]
        //public async Task<User> GetUserProvider(string email, string provider = "")
        //{
        //    //var query = from u in _context.Users
        //    //            join ul in _context.UserLogins on u.Id equals ul.IdUser
        //    //            where u.Email == email && ul.LoginProvider == provider
        //    //            select new User
        //    //            {
        //    //                Id = u.Id,
        //    //                UserName = u.UserName,
        //    //                Email = u.Email
        //    //            };

        //    //var user = await query.FirstOrDefaultAsync();
        //    var user = await _context.Users.Where(u => u.Email == email && u.Providers == provider).FirstOrDefaultAsync();

        //    return user;
        //}

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
                string email = RemoveDiacriticsAndSpaces(name) + "@gmail.com";

                var user = new User
                {
                    UserName = name,
                    Email = email.ToLower(),
                    HasPassword = _passwordManager.HashPassword("12345678")
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }

            return Ok();
        }

        private string RemoveDiacriticsAndSpaces(string input)
        {
            string normalizedString = input.Normalize(NormalizationForm.FormD);
            StringBuilder stringBuilder = new StringBuilder();

            foreach (char c in normalizedString)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark && c != ' ')
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}
