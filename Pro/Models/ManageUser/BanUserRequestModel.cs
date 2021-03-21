using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pro.Models.Enums;

namespace Pro.Models.Ban
{
    public class BanUserRequestModel : BaseRequestModel
    {
        public BanReason BanReason { get; set; }
        public string Description { get; set; }
    }
}
