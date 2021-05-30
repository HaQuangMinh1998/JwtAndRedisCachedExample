﻿using Utilities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Utilities
{
    public class JWTHelper
    {
        private static readonly object LockObject = new object();
        private static JWTHelper _instance;
        private static readonly int _weekExpiredInMinute = AppSettings.Instance.GetInt32("WeekCacheTime");
        private static readonly int _dayExpiredInMinute = AppSettings.Instance.GetInt32("DayCacheTime");
        private static readonly string _JWTSecretKey = AppSettings.Instance.GetString("JWTSecretKey");
        private static readonly string _jwtIssuer = "JWTIssuer";
        private static readonly string _jwtAudience = "JWTAudience";
        //tránh TH nhiều request từ nhiều thiết bị tạo ra nhiều jwtHelper
        public static JWTHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (LockObject)
                    {
                        if (_instance == null)
                            _instance = new JWTHelper();
                    }
                }

                return _instance;
            }
        }

        public string CreateToken( string username, string key, System.DateTime? expires, string roles)
        {
            System.DateTime issuedAt = System.DateTime.Now;
            var tokenHandler = new JwtSecurityTokenHandler();
            string issuer = AppSettings.Instance.GetString(_jwtIssuer);
            string audience = AppSettings.Instance.GetString(_jwtAudience);
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Sid, key),
                new Claim(ClaimTypes.Expired, expires?.ToString("dd/MM/yyyy HH:mm:ss.fff")),
                new Claim(ClaimTypes.Role, roles)
            });

            var now = DateTime.Now;
            var securityKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.Default.GetBytes(_JWTSecretKey));
            var signingCredentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(securityKey, Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256Signature);

            var token = (JwtSecurityToken) tokenHandler.CreateJwtSecurityToken(
                                                            issuer: issuer,
                                                            audience: audience,
                                                            subject: claimsIdentity,
                                                            notBefore: issuedAt,
                                                            expires: expires,
                                                            signingCredentials: signingCredentials
                                                        );
            var tokenString = tokenHandler.WriteToken(token);

            return tokenString;
        }

        public string GetChecksumKey(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var result = tokenHandler.ReadJwtToken(token);
            if (result != null)
            {
                var checksumObj = result.Claims.Where(x => x.Type == ClaimTypes.Sid).FirstOrDefault();
                if (checksumObj != null && checksumObj.Value != null)
                    return checksumObj.Value;
            }
            return string.Empty;
        }

        public string GenerateKeyCached(string username)
        {
            return string.Format("{0}:{1}", username.ToLower(), Guid.NewGuid().ToString());
        }


        public string CreateRefreshToken(string username, string key, System.DateTime? expires)
        {
            System.DateTime issuedAt = System.DateTime.Now;

            var tokenHandler = new JwtSecurityTokenHandler();
            string issuer = AppSettings.Instance.GetString(_jwtIssuer);
            string audience = AppSettings.Instance.GetString(_jwtAudience);
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Sid, key),
                new Claim(ClaimTypes.Expired, expires?.ToString("dd/MM/yyyy HH:mm:ss.fff")),
            });
            var now = DateTime.Now;
            var securityKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.Default.GetBytes(_JWTSecretKey));
            var signingCredentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(securityKey, Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256Signature);

            var token = (JwtSecurityToken)tokenHandler.CreateJwtSecurityToken(
                                issuer: issuer, 
                                audience: audience,
                                subject: claimsIdentity,
                                notBefore: issuedAt,
                                expires: expires,
                                signingCredentials: signingCredentials
                        );
            var tokenString = tokenHandler.WriteToken(token);

            return tokenString;
        }
      
    }
}
