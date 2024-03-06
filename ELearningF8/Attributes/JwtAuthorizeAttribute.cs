using ELearningF8.Filters;
using Microsoft.AspNetCore.Mvc;

namespace ELearningF8.Attributes
{
    public class JwtAuthorizeAttribute : TypeFilterAttribute
    {
        public JwtAuthorizeAttribute() : base(typeof(JwtAuthorizeFilter)) { }

        //public string[] Roles { get; set; }
        //public JwtAuthorizeAttribute() { }

        //public JwtAuthorizeAttribute(params string[] roles) 
        //{
        //    Roles = roles;
        //}

        //public void OnAuthorization(AuthorizationFilterContext context)
        //{

        //    if (Roles == null || Roles.Length == 0)
        //    {

        //    }
        //    else
        //    {

        //    }
        //}
    }
}
