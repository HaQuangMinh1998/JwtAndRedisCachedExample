using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ProjectX.Fr.Risk.Commons;
using ProjectX.Fr.Risk.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectX.Fr.Risk.MiddleWares
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly AppSettings _appSettings;
        private readonly ILogger<JwtMiddleware> _logger;

        public JwtMiddleware(RequestDelegate next, IOptions<AppSettings> appSettings, ILogger<JwtMiddleware> logger)
        {
            _next = next;
            _appSettings = appSettings.Value;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            Microsoft.Extensions.Primitives.StringValues queryVal;

            //token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJlbWFpbCI6InRlc3Rzc3MwMUBnbWFpbC5jb20iLCJuYW1laWQiOiI3OTg4NTJlMy0zYjE1LTQ4OTEtYmI1ZS1jMTI2MWY4ZTg3ZTQiLCJ1c2VyX2d1aWQiOiI3OTg4NTJlMy0zYjE1LTQ4OTEtYmI1ZS1jMTI2MWY4ZTg3ZTQiLCJ1c2VyX25hbWUiOiJhZG1pbiIsInVzZXJfYXZhdGFyIjoiIiwiZnVsbF9uYW1lIjoiQWRtaW5pc3RyYXRvciIsInVzZXJfZW1haWwiOiJ0ZXN0c3NzMDFAZ21haWwuY29tIiwib3JnX2d1aWQiOiJkN2QyZmQxMy1hM2IyLTQ1MjktOTBiNS00MjAxYjYzOGIzYmUiLCJleHAiOjE2NDczMjcwNzEsImlzcyI6Imlzb09ubGluZSIsImF1ZCI6Imlzb09ubGluZSJ9.Nso1ChYFAegFoWAHq1faiFt5Mwdyh1sbNgT_UZXN8qM";
            //chuyenvien
            //token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJlbWFpbCI6ImRvLnZhbi5oYXUuaXRAZ21haWwuY29tIiwibmFtZWlkIjoiZTBkNGQ3MTMtZDEzNS00NmE0LWExNjUtN2U3ZGQxNTkxNWQ2IiwidXNlcl9ndWlkIjoiZTBkNGQ3MTMtZDEzNS00NmE0LWExNjUtN2U3ZGQxNTkxNWQ2IiwidXNlcl9uYW1lIjoiY2h1eWVudmllbiIsInVzZXJfYXZhdGFyIjoiIiwiZnVsbF9uYW1lIjoiQ2h1ecOqbiB2acOqbiB0ZXN0IiwidXNlcl9lbWFpbCI6ImRvLnZhbi5oYXUuaXRAZ21haWwuY29tIiwib3JnX2d1aWQiOiJkN2QyZmQxMy1hM2IyLTQ1MjktOTBiNS00MjAxYjYzOGIzYmUiLCJjb21wYW55X2d1aWQiOiJkMjY1NGU5OS0wOTBjLTQxZGEtYTYxYi1iNGRmZmNhY2RiYjkiLCJpc19jb21wYW55IjoiVHJ1ZSIsInVuaXRfZ3VpZHMiOiI4MWM4NzkxMC03Yzk3LTRlMmItYTNmZS05NTk2YTFjYzlmYzYiLCJqb2JfcG9zaXRpb24iOiJQLlF14bqjbiBsw70ga-G7uSB0aHXhuq10ICYgxJHhuqd1IHTGsCAtIENodXnDqm4gdmnDqm4iLCJleHAiOjE2NDc1NzAxOTUsImlzcyI6Imlzb09ubGluZSIsImF1ZCI6Imlzb09ubGluZSJ9.lKenqqQBdz4kinQFt7PCHVQ5ehdjSen1nu8NjNFyE00";
            token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJlbWFpbCI6ImRvLnZhbi5oYXUuaXRAZ21haWwuY29tIiwibmFtZWlkIjoiZTBkNGQ3MTMtZDEzNS00NmE0LWExNjUtN2U3ZGQxNTkxNWQ2IiwidXNlcl9ndWlkIjoiZTBkNGQ3MTMtZDEzNS00NmE0LWExNjUtN2U3ZGQxNTkxNWQ2IiwidXNlcl9uYW1lIjoiY2h1eWVudmllbiIsInVzZXJfYXZhdGFyIjoiIiwiZnVsbF9uYW1lIjoiQ2h1ecOqbiB2acOqbiB0ZXN0IiwidXNlcl9lbWFpbCI6ImRvLnZhbi5oYXUuaXRAZ21haWwuY29tIiwib3JnX2d1aWQiOiJkN2QyZmQxMy1hM2IyLTQ1MjktOTBiNS00MjAxYjYzOGIzYmUiLCJjb21wYW55X2d1aWQiOiJkMjY1NGU5OS0wOTBjLTQxZGEtYTYxYi1iNGRmZmNhY2RiYjkiLCJpc19jb21wYW55IjoiVHJ1ZSIsInVuaXRfZ3VpZHMiOiI4MWM4NzkxMC03Yzk3LTRlMmItYTNmZS05NTk2YTFjYzlmYzYiLCJqb2JfcG9zaXRpb24iOiJQLlF14bqjbiBsw70ga-G7uSB0aHXhuq10ICYgxJHhuqd1IHTGsCAtIENodXnDqm4gdmnDqm4iLCJqb2JfcG9zaXRpb25fdW5pdF9ndWlkcyI6IjgyZDFiMDJiLTZjZTUtNGQxYi1iNmM1LWJmN2JhYzJhYTMyYiAtIDgxYzg3OTEwLTdjOTctNGUyYi1hM2ZlLTk1OTZhMWNjOWZjNiIsImV4cCI6MTY0OTI1OTQ3MywiaXNzIjoiaXNvT25saW5lIiwiYXVkIjoiaXNvT25saW5lIn0.Nb1N5kZF5wEMvJItpOwn5KFxVE6YMYJdTVMMmoX6eZY";
            if (token != null)
            {
                token = token.Replace("null", "");
                if (!string.IsNullOrEmpty(token))
                    attachUserToContext(context, token);
            }
            else if (context.Request.Query.TryGetValue("token", out queryVal))
            {
                token = queryVal.ToString().Replace("null", "");
                if (!string.IsNullOrEmpty(token))
                    attachUserToContext(context, token);
            }

            var user = context.Session.Get<EmployeeInfo>(Constants.Session.CURRENT_USER);
            if (user == null)
            {
                context.Session.Clear();
                context.Request.QueryString = new QueryString();
                context.Response.Redirect(AppServicesHelper.Config.HomeUrl);
            }

            await _next(context);
        }

        private void attachUserToContext(HttpContext context, string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_appSettings.SigningSecret);
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                if (!string.IsNullOrEmpty(jwtToken.Claims.First(x => x.Type == "user_guid").Value))
                {
                    var empInfo = new EmployeeInfo()
                    {
                        OrgGuid = jwtToken.Claims.First(x => x.Type == "org_guid").Value,
                        UserGuid = jwtToken.Claims.First(x => x.Type == "user_guid").Value,
                        UserName = jwtToken.Claims.First(x => x.Type == "user_name").Value,
                        FullName = jwtToken.Claims.First(x => x.Type == "full_name").Value,
                        Email = jwtToken.Claims.First(x => x.Type == "user_email").Value,
                        Token = token,
                        AvatarUrl = jwtToken.Claims.First(x => x.Type == "user_avatar").Value ?? "",
                        CompanyGuid = jwtToken.Claims.First(x => x.Type == "company_guid").Value,                        
                        IsCompany = jwtToken.Claims.First(x => x.Type == "is_company").Value,
                        UnitGuids = jwtToken.Claims.First(x => x.Type == "unit_guids").Value,
                        Position = jwtToken.Claims.First(x => x.Type == "job_position").Value,
                    };
                    context.Request.Headers.Add("Authorization", token);
                    var user = context.Session.Get<EmployeeInfo>(Constants.Session.CURRENT_USER);
                    if (user == null || user.Token != token)
                    {
                        context.Items["User"] = empInfo;
                        context.Session.Clear();
                        context.Session.Set(Constants.Session.CURRENT_USER, empInfo);
                    }
                }
                else
                {
                    context.Session.Clear();
                    context.Request.QueryString = new QueryString();
                    context.Response.Redirect(AppServicesHelper.Config.HomeUrl);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation("lỗi " + ex.ToString());
                // do nothing if jwt validation fails
                // user is not attached to context so request won't have access to secure routes
            }
        }
    }
}
