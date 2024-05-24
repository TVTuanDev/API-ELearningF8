using ELearningF8.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ELearningF8.Attributes
{
    public class JwtAuthorizeAttribute : TypeFilterAttribute
    {
        public string RoleName { get; set; }

        public JwtAuthorizeAttribute(string roleName = ELearningF8.Models.RoleName.Guest) 
            : base(typeof(JwtAuthorizeFilter))
        {
            RoleName = roleName;
            Arguments = new object[] { RoleName };
        }
    }

    public class JwtAuthorizeFilter : IAuthorizationFilter
    {
        public string RoleName { get; set; }
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly AppDbContext _context;

        public JwtAuthorizeFilter
            (
            string roleName,
            IHttpContextAccessor contextAccessor,
            AppDbContext appContext
            )
        {
            RoleName = roleName;
            _contextAccessor = contextAccessor;
            _context = appContext;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // Kiểm tra action của controller có [AllowAnonymous] hay không?
            var allowAnonymous = context.ActionDescriptor.EndpointMetadata
                .Any(em => em.GetType() == typeof(AllowAnonymousAttribute));
            if (allowAnonymous) return;

            var httpContext = _contextAccessor.HttpContext;
            var tokenContext = httpContext?.Request.Headers.Authorization.FirstOrDefault()?.Split(" ").Last();

            if (tokenContext == null || !httpContext!.User.Identity!.IsAuthenticated)
            {
                context.Result = new UnauthorizedObjectResult(new { Status = 401, Message = "Vui lòng đăng nhập" });
                return;
            }

            var blackList = _context.BlackLists.FirstOrDefault(bl => bl.AccessToken == tokenContext);

            if(blackList is not null)
            {
                context.Result = new BadRequestObjectResult(new { Status = 400, Message = "Access token đã được sử dụng" });
                return;
            }

            try
            {
                var token = new JwtSecurityTokenHandler().ReadJwtToken(tokenContext);

                // 1. Check kiểu ký
                if (token.SignatureAlgorithm != SecurityAlgorithms.HmacSha256)
                {
                    context.Result = new BadRequestObjectResult(new { Status = 400, Message = "Kiểu ký không chính xác" });
                    return;
                }

                // 2. Check token hết hạn chưa
                if (token.ValidTo < DateTime.UtcNow)
                {
                    context.Result = new BadRequestObjectResult(new { Status = 400, Message = "Token đã hết hạn, vui lòng đăng nhập" });
                    return;
                }

                // Check Authorize
                if (!CanAccessToAction(context.HttpContext))
                {
                    context.Result = new ObjectResult
                        (new { Status = 403, Message = "Bạn không có quyền truy cập" })
                    {
                        StatusCode = StatusCodes.Status403Forbidden
                    };
                    return;
                }

            }
            catch (ArgumentException ex)
            {
                context.Result = new BadRequestObjectResult(new { Status = 400, Message = ex.Message });
                return;
            }
        }

        private bool CanAccessToAction(HttpContext httpContext)
        {
            if (RoleName == ELearningF8.Models.RoleName.Guest) return true;

            var rolesContext = httpContext.User.FindFirstValue(ClaimTypes.Role)!.Split(",");

            foreach (var role in rolesContext)
            {
                if (RoleName == role) return true;
            }
            return false;
        }
    }
}
