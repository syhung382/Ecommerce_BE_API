using Ecommerce_BE_API.DbContext.Models.Utils;
using Ecommerce_BE_API.WebApi.Models.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using static System.Net.WebRequestMethods;

namespace Ecommerce_BE_API.WebApi.Controllers.Base
{
    public class BaseApiController : Controller
    {
        public BaseApiController()
        {
        }

        protected string CreateTokenString(
            string email,
            string fullName,
            string userId,
            string userName,
            string userSection,
            int userRole,
            string tokenKey,
            string tokenIssuer)
        {
            var claims = new[]
                {
                    new Claim(ClaimTypes.Email, email),
                    new Claim(ClaimTypes.GivenName, fullName),
                    new Claim(ClaimTypes.Sid, userId),
                    new Claim(ClaimTypes.Name, userName),
                    new Claim(ClaimTypes.Hash, userSection),
                    new Claim(ClaimTypes.Role, userRole.ToString())
                };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                tokenIssuer,
                tokenIssuer,
                claims,
                expires: DateTime.Now.AddYears(1),
                signingCredentials: creds);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return tokenString;
        }

        protected bool ValidateToken(string token, string settingKey, string issuer)
        {
            var mySecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(settingKey));
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = issuer,
                    ValidAudience = issuer,
                    IssuerSigningKey = mySecurityKey
                }, out SecurityToken validatedToken);
            }
            catch
            {
                return false;
            }
            return true;
        }

        protected ResponseResult<T> CreateResponseResult<T>(T data) where T : class
        {
            var result = new ResponseResult<T>();
            if (data != null)
            {
                result.RetCode = RetCodeEnum.Ok;
                result.RetText = RetCodeEnum.Ok.ToString();
                result.Data = data;
            }
            else
            {
                result.RetCode = RetCodeEnum.ResultNotExists;
                result.RetText = RetCodeEnum.ResultNotExists.ToString();
            }
            return result;
        }

        protected ResponseResult<T> CreateResponseApiError<T>(string errorMsg) where T : class
        {
            var result = new ResponseResult<T>(RetCodeEnum.ApiError, errorMsg, null);
            return result;
        }

        protected bool ParseQueryParamDate(string param, out DateTime valueParsed)
        {
            var result = DateTime.TryParseExact(param, "dd/MM/yyyy",
                            System.Globalization.CultureInfo.InvariantCulture,
                            System.Globalization.DateTimeStyles.None,
                            out valueParsed);
            return result;
        }
        protected bool IsPhoneNumber(string number)
        {
            return Regex.Match(number, @"^([\+]?61[-]?|[0])?[1-9][0-9]{8}$").Success;
        }

        protected bool IsValidEmail(string email)
        {
            var trimmedEmail = email.Trim();

            if (trimmedEmail.EndsWith("."))
            {
                return false; // suggested by @TK-421
            }
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == trimmedEmail;
            }
            catch
            {
                return false;
            }
        }
        protected int GetCurrentUserId()
        {
            try
            {
                var claims = User.Claims.ToList();
                var sid = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/sid").Value;
                return int.Parse(sid);
            }
            catch
            {
                return -1;
            }
        }
        protected string GetCurrentUserSession()
        {
            try
            {
                var claims = User.Claims.ToList();
                var sid = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/hash").Value;
                return sid;
            }
            catch
            {
                return null;
            }
        }

        protected string GetCurrentUserName()
        {
            try
            {
                var claims = User.Claims.ToList();
                var userName = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name").Value;
                return userName;
            }
            catch
            {
                return "";
            }
        }
        protected int GetCurrentUserRole()
        {
            try
            {
                var claims = User.Claims.ToList();
                var sid = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/role").Value;
                return int.Parse(sid);
            }
            catch
            {
                return -1;
            }
        }
        protected string GetHeaderAPIKey()
        {
            string apiKey = "";
            try
            {
                Request.Headers.TryGetValue("apiKey", out var apiKeyItem);
                if (apiKeyItem.Count > 0) apiKey = apiKeyItem;
            }
            catch
            {
                apiKey = "";
            }
            return apiKey;
        }
    }
}
