using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System.Threading.Tasks;

public class JwtHandler : AuthorizationHandler<JwtRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context, 
        JwtRequirement requirement)
    {
        if (context.Resource is HttpContext httpContext)
        {
            var request = httpContext.Request;
            var token = request.Headers["token"];
            
        }
        context.Succeed(requirement);
    }
}