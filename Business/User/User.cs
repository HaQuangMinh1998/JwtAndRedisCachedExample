using Business.User;
using Caching;
using Caching.Interfaces;
using Utilities;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Business.User
{
    public class User : IUser
    {
        private static readonly string _username = "1";
        private static readonly string _password = "1";
        private static readonly string _disName = "Ha Minh";
        private static readonly string _roles = "1,3";
        private static readonly string _jwtToken = "JWTToken";
        private static readonly string _jwtRefreshToken = "RefreshToken";
        private static readonly string _jwtLockRfToken = "JWTLockRefreshToken";
        private static readonly object ObjLocked = new object();
        private static readonly int _dayExpiredInMinute = AppSettings.Instance.GetInt32("DayCacheTime");
        private static readonly int _weekExpiredInMinute = AppSettings.Instance.GetInt32("WeekCacheTime");

        private IHttpContextAccessor _httpContextAccessor;
        private readonly ICached _cacheClient;
        public User(IHttpContextAccessor httpContextAccessor, ICached cacheClient)
        {
            this._cacheClient = cacheClient;
            this._httpContextAccessor = httpContextAccessor;
        }
        public ResponseData Login(string userName, string password, bool isSavedPassword = false, string secureCode = "")
        {
            //_httpContextAccessor.HttpContext.User.Identity.Name
            var responseData = new ResponseData();
            if (string.IsNullOrEmpty(userName))
            {
                responseData.Message = "username trống!";
            }
            if (string.IsNullOrEmpty(password))
            {
                responseData.Message = "password trống!";
            }

            password = Crypton.Encrypt(password);
            //valide login
            if (userName.Trim() == _username && password.Trim() == _password)
            {
                responseData.Data = new
                {
                    UserName = _disName,
                    Role = _roles,
                    Uid = Guid.NewGuid().ToString()
                };
                string token = string.Empty;
                string refreshToken = string.Empty;
                responseData.Success = this.JwtLogin(out token, _disName, _roles,out refreshToken);
                responseData.Token = token;
                responseData.RefreshToken = refreshToken;
                return responseData;
            }
            else
            {
                responseData.Message = "Sai username hoặc password!";
            }
            return responseData;
        }

        public ResponseData Logout()
        {
            var result = new ResponseData();
            var identity = _httpContextAccessor.HttpContext.User.Identity as ClaimsIdentity;
            var checksumKey = identity.Claims.Where(x => x.Type == ClaimTypes.Sid).FirstOrDefault();
            if (checksumKey != null && !string.IsNullOrEmpty(checksumKey.Value))
            {
                result.Success = DeleteJWTTokenOnCache( _jwtToken,checksumKey.Value) 
                    && DeleteJWTTokenOnCache(_jwtRefreshToken, checksumKey.Value);
            }
            return result;
        }
        public ResponseData RefresToken(string refreshToken)
        {
            var result = new ResponseData();
            if (!string.IsNullOrEmpty(refreshToken))
            {
                var secretKey = Encoding.ASCII.GetBytes( AppSettings.Instance.GetString("JWTSecretKey"));
                var tokenHander = new JwtSecurityTokenHandler();
                tokenHander.ValidateToken(refreshToken, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(secretKey),
                    RequireExpirationTime = true,
                    ValidateLifetime = true
                },out SecurityToken validateToken);
                var jwtToken = (JwtSecurityToken)validateToken;
                string disName = jwtToken.Claims.First(a => a.Type.Contains("Name")).Value;
                string roles = jwtToken.Claims.First(a => a.Type.Contains("Role")).Value;
                var key = JWTHelper.Instance.GenerateKeyCached(disName);
                System.DateTime expires = System.DateTime.UtcNow.AddMinutes(AppSettings.Instance.GetInt32("JWTTimeout"));
                result.Token = JWTHelper.Instance.CreateToken(disName, key, expires, roles);
            }
            return result;
        }
        private bool JwtLogin(out string token, string disName, string roles,out string refreshToken)
        {
            System.DateTime expires = System.DateTime.UtcNow.AddMinutes(AppSettings.Instance.GetInt32("JWTTimeout"));
            System.DateTime expiresAcceptToken = System.DateTime.UtcNow.AddMinutes(AppSettings.Instance.GetInt32("JWTRefreshTimeout"));
            // gen key cached
            var key = JWTHelper.Instance.GenerateKeyCached(disName);
            token = JWTHelper.Instance.CreateToken(disName, key, expires, roles);
            refreshToken = JWTHelper.Instance.CreateRefreshToken(disName, key, expiresAcceptToken);
            // lưu cache
            return this.SaveTokenOnCache(_jwtToken, key, token) &&
                this.SaveTokenOnCache(_jwtRefreshToken, key, refreshToken);

        }
        #region JWT
        public bool SaveTokenOnCache(string TokenName, string checksumKey, string acceptToken)
        {
            string keyCached = KeyCacheHelper.GenCacheKeyStatic(TokenName, checksumKey);
            return _cacheClient.Set(keyCached, acceptToken, _weekExpiredInMinute);

        }
        public bool DeleteJWTTokenOnCache(string tokenName, string key)
        {
            string keyCached = KeyCacheHelper.GenCacheKeyStatic(tokenName, key);
            return _cacheClient.Remove(keyCached);
        }
        public bool ChecksumJWTOnCache(string checksumKey)
        {
            string keyCached = KeyCacheHelper.GenCacheKeyStatic(_jwtToken, checksumKey);
            return _cacheClient.ContainsKey(keyCached);
        }
        public bool CheckExitstJWTTokenOnCache(string checksumKey, string token)
        {
            string keyCached = KeyCacheHelper.GenCacheKeyStatic(_jwtToken, checksumKey);
            var val = _cacheClient.Get(keyCached);
            if (!string.IsNullOrEmpty(val))
            {
                return token == val;
            }
            return false;
        }

        public bool CheckLockRefreshTokenOnCache(string checksumKey)
        {
            string keyCached = KeyCacheHelper.GenCacheKeyStatic(_jwtLockRfToken, checksumKey);
            return _cacheClient.Get<bool>(keyCached);
        }
        public bool SetLockRefreshTokenOnCache(string checksumKey)
        {
            lock (ObjLocked)
            {
                string keyCached = KeyCacheHelper.GenCacheKeyStatic(_jwtLockRfToken, checksumKey);
                return _cacheClient.Set(keyCached, true, 5);
            }
        }


        #endregion

    }
}
