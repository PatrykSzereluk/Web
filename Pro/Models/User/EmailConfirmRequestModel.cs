using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pro.Models.User
{
    public class EmailConfirmRequestModel : BaseRequestModel
    {
        public string UserHash { get; set; }
        public string ControlHash { get; set; }
        public string SecretValue { get; set; }
    }
}
