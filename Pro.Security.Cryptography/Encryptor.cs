namespace Pro.Security.Cryptography
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;

    public class Encryptor : IEncryptor
    {
        public string Encrypt(string password)
        {
            var hashPassword = new StringBuilder();

            using (var sha512 = SHA512.Create())
            {
                var encryptedPassword = sha512.ComputeHash(Encoding.UTF8.GetBytes(password));

                foreach (var b in encryptedPassword)
                {
                    hashPassword.Append(b.ToString("x2"));
                }

                return hashPassword.ToString();
            }

        }

        public string GenerateSalt(int count, int seed)
        {
            if (count <= 0) throw new InvalidDataException();

            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random(seed);

            var salt = new string(Enumerable.Repeat(chars, count).Select(t => t[random.Next(t.Length)]).ToArray());

            return Encrypt(salt);
        }

        public string GetHashPassword(string pass, int saltCount, int saltSeed)
        {
            var password = Encrypt(pass);

            var salt = GenerateSalt(saltCount, saltSeed);

            var fullPassword = $"{salt.Substring(0, salt.Length / 2)}{password}{salt.Substring(salt.Length / 2)}";

            return fullPassword;
        }
    }
}
