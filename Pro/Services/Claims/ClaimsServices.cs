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

        public string GetUserId()
        {
            var identity = _httpContextAccessor?.HttpContext?.User.Identity as ClaimsIdentity;

            if (identity == null)
                return string.Empty;

            return identity.FindFirst(ClaimTypes.Name)?.Value;
        }

        public bool CheckUserId(int id)
        {
            var idFromToken = GetUserId();

            if (string.IsNullOrEmpty(idFromToken))
            {
                return false;
            }

            return idFromToken == id.ToString();
        }
    }
}
