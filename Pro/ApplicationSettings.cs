using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pro
{
    public class ApplicationSettings
    {
        public string Secret { get; set; }
        public int SaltCount { get; set; }
        public int SaltSeed { get; set; }
        public string ApplicationName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string MailToTest { get; set; }
        public string EmailName { get; set; }
        public string TestEmail1 { get; set; }
        public string TestEmail2 { get; set; }
    }
}
