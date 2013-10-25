using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiAggregator.Core.Services
{
    public interface IAuthenticationService
    {
        bool Authenticate(string userName, string password);
    }
}
