using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pro.Models.User
{
    public class ChangeEmailAddressRequestModel : BaseRequestModel
    {
        public string EmailAddress { get; set; }
        public string UserHash { get; set; }
    }
}
