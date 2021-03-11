// ReSharper disable once CheckNamespace
namespace Pro.Models.DB
{
    public partial class User
    {
        public override int GetHashCode()
        {
            int hash = 19;

            if(Id > 0)
                hash = hash * 31 * Id.GetHashCode();
            
            hash = hash * 31 * Password.GetHashCode();
            hash = hash * 31 * Email.GetHashCode();
            hash = hash * 31 * UserTypeId.GetHashCode();

            if (!string.IsNullOrEmpty(UserHash))
                hash = hash * 31 * UserHash.GetHashCode();

            return hash;
        }
    }
}
