using Business.User;
using Caching;
using Caching.Interfaces;
using Utilities;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Security.Claims;

namespace Business.User
{
    public class User : IUser
    {
        private readonly string _username = "1";
        private readonly string _password = "1";
        private readonly string _disName = "Ha Minh";
        private readonly string _roles = "1,3";
        private static readonly object ObjLocked = new object();
        private static readonly int _dayExpiredInMinute = AppSettings.Instance.GetInt32("DayCacheTime");
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
                responseData.Success = this.JwtLogin(out token, _disName, _roles);
                responseData.Token = token;
                return responseData;
            }
            else {
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
                result.Success = DeleteJWTTokenOnCache(checksumKey.Value);
            }
            return result;
        }
        private bool JwtLogin(out string token,string disName, string roles)
        {
            //set the time when it expires
            System.DateTime expires = System.DateTime.UtcNow.AddMinutes(AppSettings.Instance.GetInt32("JWTTimeout"));
            // gen checksumKey
            var key = JWTHelper.Instance.GenerateKeyCached(disName);
            token = JWTHelper.Instance.CreateToken(0, disName, null, key, expires, roles);

            // lưu key cached và token vào cache
            return SaveJWTTokenOnCache(key, token);

        }
        #region JWT
        public bool SaveJWTTokenOnCache(string key, string token)
        {
            string keyCached = KeyCacheHelper.GenCacheKeyStatic("JWTToken", key);
            return _cacheClient.Set(keyCached, token, _dayExpiredInMinute);

        }

        public bool DeleteJWTTokenOnCache(string key)
        {
            string keyCached = KeyCacheHelper.GenCacheKeyStatic("JWTToken", key);
            return _cacheClient.Remove(keyCached);
        }
        public bool ChecksumJWTOnCache(string checksumKey)
        {
            string keyCached = KeyCacheHelper.GenCacheKeyStatic("JWTToken", checksumKey);
            return _cacheClient.ContainsKey(keyCached);
        }
        public bool CheckExitstJWTTokenOnCache(string checksumKey, string token)
        {
            string keyCached = KeyCacheHelper.GenCacheKeyStatic("JWTToken", checksumKey);
            var val = _cacheClient.Get(keyCached);
            if (!string.IsNullOrEmpty(val))
            {
                return token == val;
            }
            return false;
        }

        public bool CheckLockRefreshTokenOnCache(string checksumKey)
        {
            string keyCached = KeyCacheHelper.GenCacheKeyStatic("JWTLockRefreshToken", checksumKey);
            return _cacheClient.Get<bool>(keyCached);
        }
        public bool SetLockRefreshTokenOnCache(string checksumKey)
        {
            lock (ObjLocked)
            {
                string keyCached = KeyCacheHelper.GenCacheKeyStatic("JWTLockRefreshToken", checksumKey);
                return _cacheClient.Set(keyCached, true, 5);
            }
        }


        #endregion

    }
}
