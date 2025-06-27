using Ecommerce_BE_API.DbContext.Models.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;

public class AuthorizeRoleAttribute : Attribute, IAuthorizationFilter
{
    private readonly AdminRoleEnum[] _allowedRoles;

    public AuthorizeRoleAttribute(params AdminRoleEnum[] roles)
    {
        _allowedRoles = roles;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;

        if (!user.Identity?.IsAuthenticated ?? true)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var roleClaim = user.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Role);
        if (roleClaim == null || !int.TryParse(roleClaim.Value, out int roleValue))
        {
            context.Result = new ForbidResult("Missing or invalid role claim.");
            return;
        }

        var currentUserRole = (AdminRoleEnum)roleValue;
        if (!_allowedRoles.Contains(currentUserRole))
        {
            context.Result = new ForbidResult("Bạn không có quyền truy cập.");
        }
    }
}