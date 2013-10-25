using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiAggregator.Core.Services
{
    public interface IHashingService
    {
        string GenerateApiKey();
        string GenerateQRSecret();
        string GenerateSalt();
        string HashPassword(string password, string salt);
        bool CheckPassword(string plaintext, string hash);
    }
}
