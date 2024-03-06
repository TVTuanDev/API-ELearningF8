using ELearningF8.Data;
using ELearningF8.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace ELearningF8.Filters
{
    public class JwtAuthorizeFilter : IAuthorizationFilter
    {
        private readonly IConfiguration _conf;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly AppDbContext _context;

        public JwtAuthorizeFilter
            (
            IConfiguration conf,
            IHttpContextAccessor contextAccessor,
            AppDbContext appContext
            )
        {
            _conf = conf;
            _contextAccessor = contextAccessor;
            _context = appContext;
        }

        public async void OnAuthorization(AuthorizationFilterContext context)
        {
            var httpContext = _contextAccessor.HttpContext;
            // Kiểm tra xem có cookie "token" trong request không
            if (httpContext.Request.Cookies.TryGetValue("token", out string? tokenJson))
            {
                // Nếu có, giải mã chuỗi JSON để lấy token
                TokenVM token = JsonConvert.DeserializeObject<TokenVM>(tokenJson);
                var tokenHandler = new JwtSecurityTokenHandler();
                var secretKey = Encoding.UTF8.GetBytes(_conf["Jwt:SecretKey"] ?? "");
                try
                {
                    tokenHandler.ValidateToken(token.AccessToken, new TokenValidationParameters
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
                        if (!result)
                        {
                            context.Result = new BadRequestObjectResult("Sai định dạng token");
                            return;
                        }
                    }

                    // 2.Check access token đã hết hạn chưa
                    if (jwtToken.ValidTo > DateTime.UtcNow)
                    {
                        // Access token vẫn còn hạn
                        context.Result = new OkResult();
                        return;
                    }

                    // 3.Check refresh token có trong db hay không
                    //var refreshTokenDb = _context.RefreshTokens.FirstOrDefault(rt => rt.Token == token.RefreshToken);
                    //if (refreshTokenDb != null)
                    //{
                    //    context.Result = new BadRequestObjectResult("Refresh token không hợp lệ");
                    //    return;
                    //}

                    // 4.Check refresh token đã được sử dụng hay chưa
                    //if (refreshTokenDb?.IsUsed == true)
                    //{
                    //    context.Result = new BadRequestObjectResult("Refresh token đã được sử dụng");
                    //    return;
                    //}

                    // 5.Check access token id == JwtId trong RefreshToken không
                    // Lấy id của access token
                    var jti = jwtToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;
                    //if (refreshTokenDb.JwtId != jti)
                    //{
                    //    context.Result = new BadRequestObjectResult("Refresh token và access token không cùng 1 cặp");
                    //    return;
                    //}

                    // Cập nhật refresh token này đã được sử dụng
                    //refreshTokenDb.IsUsed = true;
                    await _context.SaveChangesAsync();

                    // Tạo mới TokenModel
                    var userId = int.Parse(jwtToken.Claims.First().Value);
                    context.HttpContext.Items["userId"] = userId;
                    var user = await _context.Users.SingleOrDefaultAsync(u => u.Id == userId);
                }
                catch (Exception)
                {
                    context.Result = new ObjectResult("Lỗi đăng nhập")
                    {
                        StatusCode = 401
                    };
                }
            }
            else
            {
                context.Result = new ObjectResult("Yêu cầu đăng nhập")
                {
                    StatusCode = 401
                };
                //context.Result = new UnauthorizedResult();
                return;
            }
        }
    }
}
