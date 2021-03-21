using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pro.Models.Enums;

namespace Pro.Models.ManageUser
{
    public class ChangeUserRoleRequestModel : BaseRequestModel
    {
        public RoleType RoleType { get; set; }
    }
}
