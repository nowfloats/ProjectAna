using ANAConversationPlatform.Helpers;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ANAConversationPlatform.Attributes
{
    public class BasicAuthenticationAttribute : ActionFilterAttribute
    {
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (Utils.BasicAuth == null || string.IsNullOrWhiteSpace(Utils.BasicAuth.APIKey) || string.IsNullOrWhiteSpace(Utils.BasicAuth.APISecret))
            {
                await next();
                return;
            }

            var headers = context.HttpContext.Request.Headers;
            var authHeader = headers["Authorization"].FirstOrDefault();
            if (string.IsNullOrWhiteSpace(authHeader))
            {
                context.HttpContext.Response.StatusCode = 401;
                return;
            }
            var savedAuthBase64 = Utils.BasicAuth.GetBase64();
            var givenAuthBase64 = authHeader.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).Skip(1).FirstOrDefault()?.Trim();
            if (givenAuthBase64 != savedAuthBase64)
            {
                context.HttpContext.Response.StatusCode = 401;
                return;
            }
            await next();
        }
    }
}
