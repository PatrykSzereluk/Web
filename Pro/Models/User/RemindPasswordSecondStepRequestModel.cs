using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pro.Models.User
{
    public class RemindPasswordSecondStepRequestModel : BaseRequestModel
    {
        public string NewPassword { get; set; }
        public string ControlHash { get; set; }
    }
}
