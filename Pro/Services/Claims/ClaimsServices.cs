using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Pro.Services.Claims
{
    public class ClaimsServices : IClaimsService
    {

        private readonly IHttpContextAccessor _httpContextAccessor;

        public ClaimsServices(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public bool CheckUserId(int id)
        {
            var identity = _httpContextAccessor?.HttpContext?.User.Identity as ClaimsIdentity;

            if (identity == null)
                return false;

            var idFromToken = identity.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(idFromToken))
            {
                return false;
            }

            if (idFromToken != id.ToString())
                return false;

            return true;
        }
    }
}
