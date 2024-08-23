using finalproject.models;
using finalproject.Models;
using finalproject.Repositories;

namespace finalproject.CustomMiddleware
{
    public class AuthorizeUserMiddleware : IMiddleware
    {
        private readonly IRepositoryWrapper _repositoryWrapper;
        public AuthorizeUserMiddleware(IRepositoryWrapper repositoryWrapper)
        {
            _repositoryWrapper = repositoryWrapper;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var endpoint = context.Request.Path.ToString().ToLower();
            if (endpoint == "/api/user/login" || endpoint == "/api/publicrequest/heartbeat" || endpoint == "/api/user/getuser" || endpoint == "/api/user/updateuser")
            {
                await next(context);
                return;
            }
            else
            {
                string[] pathArray = endpoint.Split('/');
                if (pathArray.Length > 3)
                {
                    string endpointName = pathArray[2] + "/" + pathArray[3];
                    // retrieve user ID from claim
                    int userID = int.Parse(context.User?.FindFirst(x => x.Type == "sid")?.Value ?? "0");
                    User? userObj = await _repositoryWrapper.User.FindByID(userID);
                    if (userObj != null)
                    {
                        int userLevelID = userObj.userlevel_id;
                        UserLevelMenu? userLevelMenuObj = await _repositoryWrapper.UserLevelMenu.GetUserLevelMenu(userLevelID, endpointName);
                        if (userLevelMenuObj != null)
                        {
                            await next(context);
                            return;
                        }
                        else
                        {
                            context.Response.StatusCode = 403;
                            await context.Response.WriteAsync("Forbidden");
                            return;
                        }
                    }
                    else
                    {
                        context.Response.StatusCode = 401;
                        await context.Response.WriteAsync("Unauthorized");
                        return;
                    }
                }
                else
                {
                    // Handle the case where the path does not have enough segments
                    context.Response.StatusCode = 400; // Bad Request
                    await context.Response.WriteAsync("Invalid endpoint format");
                    return;
                }
            }
        }
    }

    public static class RequestAuthorizeUserMiddlewareExtensions
    {
        public static IApplicationBuilder UseAuthorizeUser(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuthorizeUserMiddleware>();
        }
    }
}
