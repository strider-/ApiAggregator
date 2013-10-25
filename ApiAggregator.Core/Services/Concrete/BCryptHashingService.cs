using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DevOne.Security.Cryptography.BCrypt;

namespace ApiAggregator.Core.Services.Concrete
{
    public class BCryptHashingService : IHashingService
    {
        const int WORK_FACTOR = 10;

        public string GenerateSalt()
        {
            return BCryptHelper.GenerateSalt(WORK_FACTOR);
        }

        public string HashPassword(string password, string salt)
        {
            return BCryptHelper.HashPassword(password, salt);
        }

        public bool CheckPassword(string plaintext, string hash)
        {
            return BCryptHelper.CheckPassword(plaintext, hash);
        }

        public string GenerateQRSecret()
        {
            var secret = new byte[16];
            new RNGCryptoServiceProvider().GetNonZeroBytes(secret);
            return Convert.ToBase64String(secret);
        }

        public string GenerateApiKey()
        {
            var randomBytes = Encoding.UTF8.GetBytes(GenerateSalt());
            var hash = MD5.Create().ComputeHash(randomBytes);
            var sb = new StringBuilder(hash.Length * 2);

            foreach(var b in hash)
            {
                sb.AppendFormat("{0:x2}", b);
            }

            return sb.ToString();
        }
    }
}