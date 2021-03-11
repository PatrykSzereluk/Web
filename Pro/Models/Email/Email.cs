using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pro.Models.Email
{
    public class Email
    {
        public string Subject { get; set; }
        public string Body { get; set; }

        public List<Recipient> Recipients;
    }
}
