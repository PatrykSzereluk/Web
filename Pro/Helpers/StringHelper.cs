namespace Pro.Helpers
{
    using System;
    using System.Net.Mail;

    public static class StringHelper
    {
        public static bool HasOnlyDigitOrLetter(this string @string)
        {
            foreach (var character in @string)
            {
                if (!char.IsLetterOrDigit(character))
                    return false;
            }

            return true;
        }

        public static bool HasValidCharacterEmail(this string email)
        {
            try
            {
                MailAddress mailAddress = new MailAddress(email);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool HasValidCharacterPassword(this string password)
        {
            return !password.HasSurrogateCharacter();
        }

        public static bool HasSurrogateCharacter(this string @string)
        {
            foreach (var item in @string)
            {
                if (char.IsSurrogate(item))
                    return true;
            }
            return false;
        }
    }
}
