using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApiAggregator.Core.Data;

namespace ApiAggregator.Core.Services.Concrete
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IConfigurationRepository _repo;
        private readonly IHashingService _hasher;

        public AuthenticationService(IConfigurationRepository session, IHashingService hasher)
        {
            _repo = session;
            _hasher = hasher;
        }

        public bool Authenticate(string userName, string password)
        {
            var option = _repo.All().Single();
            if(!option.RequireLogin)
            {
                return true;
            }

            if(option == null)
            {
                return false;
            }

            return _hasher.CheckPassword(password, option.Password);
        }
    }
}
