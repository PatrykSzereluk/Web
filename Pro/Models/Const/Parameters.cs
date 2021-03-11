using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pro.Models.Const
{
    public static class EmailParameters
    {
        public const string Login    = "r-Login";
        public const string Email    = "r-Email";
        public const string AddressSite = "r-AddressSite";
        public const string UserHash = "r-UserHash";
        public const string ControlHash = "r-ControlHash";
    }

    public static class EmailTemplates
    {
        public const string EmailRegisterTemplate = "RegisterTemplate.html";
        public const string EmailRemindTemplate = "RemindPasswordTemplate.html";
    }

    public static class Definition
    {
        public const string Resources = "Resources";
        public const string EmailTemplates = "EmailTemplates";
    }

    public static class EmailSubject
    {
        public const string Register = "Welcome to our community";
        public const string RemindPassword = "Remind your password";
    }

}
