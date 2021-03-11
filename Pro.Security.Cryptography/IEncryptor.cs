using System;

namespace Pro.Security.Cryptography
{
    public interface IEncryptor
    {
        string Encrypt(string password);
        string GenerateSalt(int count, int seed);

        string GetHashPassword(string pass, int saltCount, int saltSeed);
    }
}
