using Business.User;
using Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace ActionFilter
{
    public class CustomizeAuthorizeAttribute : TypeFilterAttribute
    {
        public CustomizeAuthorizeAttribute(params int[] _roles) : base(typeof(CustomizeAuthorizeFilter))
        {
            Arguments = new object[] { _roles };
        }
        public class CustomizeAuthorizeFilter : IAsyncActionFilter
        {
            private static readonly string _jwtToken = "JWTToken";
            private static readonly string _jwtIssuer = "JWTIssuer";
            private static readonly string _jwtAudience = "JWTAudience";
            private readonly IUser _userService;
            private int[] Roles { get; set; }
            public CustomizeAuthorizeFilter(int[] _role, IUser userService)
            {
                Roles = _role;
                _userService = userService;
            }
            public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                var token = string.Empty;
                if (!TryRetrieveToken(context.HttpContext.Request, out token))
                {
                    context.HttpContext.Response.StatusCode = HttpStatusCode.Unauthorized.GetHashCode();
                    await context.HttpContext.Response.WriteAsync("Jwt token không hợp lệ!");
                    return;
                }

                try
                {
                    string sec = AppSettings.Instance.GetString("JWTSecretKey");
                    var now = DateTime.Now;
                    var securityKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.Default.GetBytes(sec));


                    SecurityToken securityToken;
                    JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
                    string issuer = AppSettings.Instance.GetString(_jwtIssuer);
                    string audience = AppSettings.Instance.GetString(_jwtAudience);

                    // Đối tượng chưa check expire để lấy thông tin 
                    TokenValidationParameters validationParameters = new TokenValidationParameters()
                    {
                        ValidAudience = audience,
                        ValidIssuer = issuer,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        LifetimeValidator = this.LifetimeValidator,
                        IssuerSigningKey = securityKey,
                    };

                    // lấy key cached
                    var currentPrincipal = handler.ValidateToken(token, validationParameters, out securityToken);
                    if (currentPrincipal != null)
                    {
                        var checksumObj = currentPrincipal.Claims.Where(x => x.Type == ClaimTypes.Sid).FirstOrDefault();
                        if (checksumObj != null)
                        {
                            var keyCached = checksumObj.Value;
                            if (!string.IsNullOrEmpty(keyCached))
                            {
                                var expiredObj = currentPrincipal.Claims.Where(x => x.Type == ClaimTypes.Expired).FirstOrDefault();
                                if (expiredObj != null && !string.IsNullOrEmpty(expiredObj.Value))
                                {
                                    // kiểm tra trong cache có tồn tài key cached token không
                                    //if (_userService.ChecksumJWTOnCache(keyCached))
                                    if(true)
                                    {
                                        var roles = string.Empty;
                                        var rolesObj = currentPrincipal.Claims.Where(x => x.Type == ClaimTypes.Role).FirstOrDefault();
                                        if (rolesObj != null && !string.IsNullOrEmpty(rolesObj.Value)) roles = rolesObj.Value;

                                        // kiểm tra xem token có tồn tại trong cache không
                                        //if (_userService.CheckExitstJWTTokenOnCache(keyCached, token))
                                        if(true)
                                        {
                                            //Kiểm tra xem token đã bị expire chưa
                                            long expired = expiredObj.Value.ToLong();
                                            //chuyển thời gian là số sang dạng datetime 
                                            JWTHelper jwtHelper = new JWTHelper();
                                            var expiredDate = jwtHelper.UnixTimeStampToDateTime(expired);

                                            // nếu token expired thì gen token mới
                                            if (System.DateTime.Now >= expiredDate)
                                            {
                                                while (_userService.CheckLockRefreshTokenOnCache(keyCached))
                                                {
                                                    Thread.Sleep(1000);
                                                }
                                                _userService.SetLockRefreshTokenOnCache(keyCached);


                                                var userName = currentPrincipal.Identity.Name;
                                                //thêm thời gian expire
                                                System.DateTime expires = System.DateTime.Now.AddMinutes(AppSettings.Instance.GetInt32("JWTTimeout"));

                                                var newToken = JWTHelper.Instance.CreateToken(userName, keyCached, expires, roles);
                                                _userService.SaveTokenOnCache(_jwtToken, keyCached, newToken);

                                                var Identity = currentPrincipal.Identity as ClaimsIdentity;
                                                Identity.AddClaim(new Claim(ClaimTypes.Authentication, newToken));
                                                currentPrincipal.AddIdentity(Identity);
                                            }

                                            context.HttpContext.User = currentPrincipal;

                                            if (!CheckRoles(roles))
                                            {
                                                context.HttpContext.Response.StatusCode = HttpStatusCode.MethodNotAllowed.GetHashCode();
                                                return;
                                            }

                                            await next();
                                            return;
                                        }
                                        else
                                        {
                                            context.HttpContext.Response.StatusCode = HttpStatusCode.Unauthorized.GetHashCode();
                                        }
                                    }
                                }
                            }
                        }
                    }

                   // throw new SecurityTokenValidationException();
                }
                catch (SecurityTokenValidationException e)
                {
                    context.HttpContext.Response.StatusCode = HttpStatusCode.Unauthorized.GetHashCode();
                }
                catch (Exception ex)
                {
                    context.HttpContext.Response.StatusCode = ex.Message.Contains("JWT") ? HttpStatusCode.Unauthorized.GetHashCode() : HttpStatusCode.InternalServerError.GetHashCode();
                }

                return;
            }

            private bool CheckRoles(string lstRoles)
            {
                if (Roles.Length > 0)
                {
                    if (lstRoles != null && !string.IsNullOrEmpty(lstRoles))
                    {
                        var lstRolesStr = lstRoles.Split(',');
                        if (lstRolesStr.Length > 0)
                        {
                            var roles = lstRolesStr.Select(x => x.ToInt()).Where(x => x > 0).ToArray();
                            foreach (var permission in Roles)
                            {
                                if (roles.Contains(permission)) return true;
                            }
                        }
                    }
                    return false;
                }
                return true;
            }

            private bool LifetimeValidator(System.DateTime? notBefore, System.DateTime? expires, SecurityToken securityToken, TokenValidationParameters validationParameters)
            {
                if (expires != null)
                {
                    return true;
                }
                return false;
            }

            private bool TryRetrieveToken(HttpRequest request, out string token)
            {
                token = null;
                StringValues authzHeaders;
                if (!request.Headers.TryGetValue("Authorization", out authzHeaders) || authzHeaders.Count() > 1)
                {
                    return false;
                }
                var bearerToken = authzHeaders.ElementAt(0);
                token = bearerToken.StartsWith("Bearer ") ? bearerToken.Substring(7) : bearerToken;
                return true;
            }
        }

    }
}
