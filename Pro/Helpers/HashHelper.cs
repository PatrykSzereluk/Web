namespace Pro.Helpers
{
    using Models.DB;

    public static class HashHelper
    {
        public static bool CheckCount(this int count)
        {
            return count >= 2;
        }

        public static string GetHash(this User user)
        {
            return user.GetHashCode().ToString("x2");
        }

    }
}
