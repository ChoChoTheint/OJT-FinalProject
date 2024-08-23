using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;


namespace finalproject.Util
{
    public static class GlobalFunction
    {
        private const int PBKDF2_ITERATIONS = 1000;
        private const int HASH_BYTE_SIZE = 24;
        private const int SALT_BYTE_SIZE = 24;
        public static string ComputeHash(string salt, string password)
        {
            var saltBytes = Convert.FromBase64String(salt);
            using (var rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, saltBytes, PBKDF2_ITERATIONS, HashAlgorithmName.SHA256))
                return Convert.ToBase64String(rfc2898DeriveBytes.GetBytes(HASH_BYTE_SIZE));
        }


        public static string GenerateSalt()
        {
            var rng = RandomNumberGenerator.Create();
            var buff = new byte[SALT_BYTE_SIZE];
            rng.GetBytes(buff);
            return Convert.ToBase64String(buff);
        }


        public static string CreateJWTToken(Claim[] claims)
        {
            var appsettingbuilder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
            var Configuration = appsettingbuilder.Build();
            string encodedJwt = "";
            try
            {
                var now = DateTime.UtcNow;
                TimeSpan expiration = TimeSpan.FromMinutes(Convert.ToDouble(Configuration.GetSection("TokenAuthentication:TokenExpiry").Value));
                SigningCredentials singingKey = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration.GetSection("TokenAuthentication:SecretKey").Value)), SecurityAlgorithms.HmacSha256);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Audience = Configuration.GetSection("TokenAuthentication:Audience").Value,
                    Issuer = Configuration.GetSection("TokenAuthentication:Issuer").Value,
                    Subject = new ClaimsIdentity(claims),
                    NotBefore = now,
                    IssuedAt = UnixTimeStampToDateTime(Int32.Parse(claims.First(claim => claim.Type == "iat").Value)),
                    Expires = now.Add(expiration),
                    SigningCredentials = singingKey,
                };
                var handler = new JwtSecurityTokenHandler();
                encodedJwt = handler.CreateEncodedJwt(tokenDescriptor);


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return encodedJwt;
        }


        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dateTime;
        }


        public static Claim[] CreateClaim(int userid, string issueDate)
        {
            var claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sid, userid.ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, issueDate, ClaimValueTypes.Integer64),
            };
            return claims;
        }
    }
}


