using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using System.Text;

namespace AdminLte.Helpers
{
    public class CustomAuthFilter : AuthorizeAttribute, IAuthorizationFilter
    {
        private string type;
        public CustomAuthFilter(string type)
        {
            this.type = type;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User as ClaimsPrincipal;
            byte[] bytes;
            if(context.HttpContext.Session.TryGetValue(type, out bytes))
            {
                string value = Encoding.ASCII.GetString(bytes);
                if(value == "1")
                {

                } 
                else
                {
                    context.Result = new RedirectResult("~/Identity/Account/AccessDenied");
                }
            } 
            else
            {
                context.Result = new RedirectResult("~/Identity/Account/AccessDenied");
            }
        }

    }

}
