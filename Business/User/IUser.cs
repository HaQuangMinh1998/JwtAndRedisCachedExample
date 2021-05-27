using Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.User
{
    public interface IUser
    {
        ResponseData Login(string userName, string password, bool isSavedPassword = false, string secureCode = "");
        ResponseData Logout();
        bool ChecksumJWTOnCache(string keyCached);
        bool CheckExitstJWTTokenOnCache(string keyCached, string token);
        bool SaveJWTTokenOnCache(string tokenName, string key, string token);
        bool CheckLockRefreshTokenOnCache(string keyCached);
        bool SetLockRefreshTokenOnCache(string checksumKey);
        bool DeleteJWTTokenOnCache(string tokenName, string key);
    }
}
