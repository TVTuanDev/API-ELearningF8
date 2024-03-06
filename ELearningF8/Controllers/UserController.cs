using ELearningF8.Data;
using ELearningF8.Models;
using ELearningF8.Services;
using ELearningF8.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ELearningF8.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly AppDbContext _context;
        private readonly PasswordManager _passwordManager;
        private readonly MailHandleServices _mailService;
        private readonly IConfiguration _conf;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IDistributedCache _cache;

        public UserController
            (
            ILogger logger, 
            AppDbContext context, 
            PasswordManager passwordManager, 
            MailHandleServices mailService, 
            IConfiguration conf, 
            IHttpContextAccessor contextAccessor, 
            IDistributedCache cache
            )
        {
            _logger = logger;
            _context = context;
            _passwordManager = passwordManager;
            _mailService = mailService;
            _conf = conf;
            _contextAccessor = contextAccessor;
            _cache = cache;
        }

        [HttpPost("/register")]
        public async Task<IActionResult> RegisterAsync(RegisterVM model)
        {
            try
            {
                //returnUrl ??= Url.Content("~/");
                if (!ModelState.IsValid) return BadRequest(new { Message = "Nhập sai thông tin" });
                if (await _mailService.CheckMail(model.Email)) return BadRequest(new { Message = "Email đã được sử dụng" });

                var verifyEmail = await VerifyEmailAsync(model.Email, model.Code);
                if (verifyEmail)
                {
                    var user = new User
                    {
                        UserName = model.UserName,
                        Email = model.Email,
                        HasPassword = _passwordManager.HashPassword(model.Password),
                        CreateAt = DateTime.UtcNow
                    };
                    await _context.AddAsync(user);
                    await _context.SaveChangesAsync();

                    return Ok(user);
                }

                return BadRequest(new { Message = "Mã code hết hạn, vui lòng lấy mã mới" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPost("/login")]
        public async Task<IActionResult> LoginAsync(LoginVM model)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(new { Message = "Nhập sai thông tin" });
                if (!(await _mailService.CheckMail(model.Email))) return BadRequest(new { Message = "Email chưa được đăng ký" });

                var verifyEmail = await VerifyEmailAsync(model.Email, model.Code);
                if (verifyEmail)
                {
                    var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
                    if (user != null)
                    {
                        var checkPassword = _passwordManager.VerifyPassword(model.Password, user.HasPassword);
                        if (checkPassword)
                        {
                            // Cấp accessToken và refreshToken
                            var token = GenerateToken(user);

                            ////Tạo cookie lưu token
                            //var httpContext = _contextAccessor.HttpContext;
                            //var cookieOptions = new CookieOptions
                            //{
                            //    // Thời gian hết hạn của cookie
                            //    Expires = DateTime.UtcNow.AddMinutes(1),
                            //    // Cookie được áp dụng với toàn bộ trang web với đường dẫn "/"
                            //    Path = "/"
                            //};
                            //// Lưu cookie
                            //string tokenJson = JsonConvert.SerializeObject(token);
                            //httpContext.Response.Cookies.Append("token", tokenJson, cookieOptions);

                            return Ok(token);
                        }
                        return BadRequest("Mật khẩu không chính xác");
                    }
                    return BadRequest("Email không chính xác");
                }
                return BadRequest("Mã code hết hạn, vui lòng lấy mã mới");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        private TokenVM GenerateToken(User user)
        {
            // Tạo đối tượng để tạo mới jwt hoặc xác minh các jwt
            var tokenHandler = new JwtSecurityTokenHandler();
            // Lấy khóa bí mật trong file appsetting.json
            // Khóa này được sử dụng để ký và xác minh jwt
            var secretKey = Encoding.UTF8.GetBytes(_conf["Jwt:SecretKey"] ?? "");
            // Tạo access token chứa thông tin jwt
            var accessTokenDesc = new SecurityTokenDescriptor
            {
                // Chứa thông tin về người dùng được định danh trong JWT
                Subject = new ClaimsIdentity(new[]
                {
                    // Giá trị định danh duy nhất
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    // Tạo id cho access token
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(ClaimTypes.Name, user.UserName)
                }),
                // Thời gian hết hạn của jwt
                Expires = DateTime.UtcNow.AddSeconds(30),
                // Loại xác thực để ký jwt
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha512Signature)
            };
            // Tạo token dựa trên thông tin của tokenDescriptior
            var tokenAccess = tokenHandler.CreateToken(accessTokenDesc);
            // Chuyển đổi jwt thành string
            var accessToken = tokenHandler.WriteToken(tokenAccess);

            // Tạo refresh token chứa thông tin jwt
            var refreshTokenDesc = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.NameIdentifier, tokenAccess.Id.ToString()),
                }),
                Expires = DateTime.UtcNow.AddMinutes(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha512Signature)
            };

            // Refresh token
            var tokenRefresh = tokenHandler.CreateToken(refreshTokenDesc);
            var refreshToken = tokenHandler.WriteToken(tokenRefresh);

            // Lưu token vào db
            //var refreshTokenDb = new RefreshToken
            //{
            //    IdUser = user.Id,
            //    JwtId = tokenAccess.Id,
            //    Token = refreshToken,
            //    IsUsed = false,
            //    IssuedAt = DateTime.UtcNow,
            //    ExpiredAt = DateTime.UtcNow.AddDays(1)
            //};

            var tokenModel = new TokenVM
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };

            ////Tạo cookie lưu token
            //var httpContext = _contextAccessor.HttpContext;
            //// Nếu tồn tại cookie token thì cập nhật lại 
            //if (httpContext.Request.Cookies.ContainsKey("token"))
            //{
            //    var oldCookie = httpContext.Request.Cookies["token"];
            //}
            //var cookieOptions = new CookieOptions
            //{
            //    // Thời gian hết hạn của cookie
            //    Expires = DateTime.UtcNow.AddMinutes(1),
            //    // Cookie được áp dụng với toàn bộ trang web với đường dẫn "/"
            //    Path = "/"
            //};
            //// Lưu cookie
            //string tokenJson = JsonConvert.SerializeObject(tokenModel);
            //httpContext.Response.Cookies.Append("token", tokenJson, cookieOptions);

            return tokenModel;
        }

        private string GenerateRefreshToken()
        {
            // Tạo chuỗi ngẫu nhiên
            var random = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(random);

                return Convert.ToBase64String(random);
            }
        }

        [HttpPost("/refresh-token")]
        public async Task<IActionResult> RefreshToken(TokenVM model)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var secretKey = Encoding.UTF8.GetBytes(_conf["Jwt:SecretKey"] ?? "");
            try
            {
                tokenHandler.ValidateToken(model.AccessToken, new TokenValidationParameters
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

                // 1.Check access token có đúng kiểu ký
                if (jwtToken is JwtSecurityToken)
                {
                    var result = jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512, StringComparison.InvariantCultureIgnoreCase);
                    if (!result) return BadRequest("Sai định dạng token");
                }

                // 2.Check access token đã hết hạn chưa
                if (jwtToken.ValidTo > DateTime.UtcNow)
                {
                    // Access token vẫn còn hạn
                    return BadRequest("Access token chưa hết hạn");
                }

                //// 3.Check refresh token có trong db hay không
                //var refreshTokenDb = _context.RefreshTokens.FirstOrDefault(rt => rt.Token == model.RefreshToken);
                //if (refreshTokenDb != null) return BadRequest("Refresh token không hợp lệ");

                //// 4.Check refresh token đã được sử dụng hay chưa
                //if (refreshTokenDb?.IsUsed == true) return BadRequest("Refresh token đã được sử dụng");

                //// 5.Check access token id == JwtId trong RefreshToken không
                //// Lấy id của access token
                //var jti = jwtToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;
                //if (refreshTokenDb?.JwtId != jti) return BadRequest("Refresh token và access token không cùng 1 cặp");

                //// Cập nhật refresh token này đã được sử dụng
                //refreshTokenDb.IsUsed = true;
                //await _context.SaveChangesAsync();

                //// Tạo mới TokenModel
                //var user = await _context.Users.SingleOrDefaultAsync(u => u.Id == refreshTokenDb.IdUser);
                //var token = GenerateToken(user);

                //return Ok(token);
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        // [NonAction] // Dành cho function ko phải action api 
        private async Task<bool> VerifyEmailAsync(string email, string code)
        {
            // Check email và code
            byte[] getCache = _cache.Get(email);
            if (getCache != null)
            {
                var cacheData = Encoding.UTF8.GetString(getCache);
                if (cacheData == code)
                {
                    _cache.Remove(email);
                    return true;
                }
            }

            //var codeMailDb = _context.SendMailCodes
            //    .Where(e => e.Email == email && e.Code == code).FirstOrDefault();
            //if (codeMailDb != null && codeMailDb.ExpiredAt > DateTime.UtcNow)
            //{
            //    codeMailDb.IsUsed = true;
            //    codeMailDb.ExpiredAt = DateTime.UtcNow;

            //    await _context.SaveChangesAsync();
            //    return true;
            //}
            return false;
        }
    }
}
