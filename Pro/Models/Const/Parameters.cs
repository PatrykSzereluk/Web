namespace Pro.Models.Const
{
    public static class EmailParameters
    {
        public const string Login    = "r-Login";
        public const string Email    = "r-Email";
        public const string AddressSite = "r-AddressSite";
        public const string UserHash = "r-UserHash";
        public const string ControlHash = "r-ControlHash";
        public const string RandomString = "r-RandomString";
        public const string FbIcon = "i-fb";
        public const string IgIcon = "i-ig";
        public const string TwIcon = "i-tw";
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
        public const string Icons = "Icons";
    }

    public static class Icon
    {
        public const string Facebook = "fb.png";
        public const string Instagram = "ig.png";
        public const string Tweeter = "tw.png";
    }

    public static class EmailSubject
    {
        public const string Register = "Welcome to our community";
        public const string RemindPassword = "Remind your password";
    }

}
