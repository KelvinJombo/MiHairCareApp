using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiHairCareApp.Application.Interfaces.Services
{
    public interface IJwtTokenService
    {
        string GenerateToken(string userId, string email, string role);
    }

}
