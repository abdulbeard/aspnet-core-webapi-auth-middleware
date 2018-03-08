using System.Security.Claims;

namespace TokenAuth.Auth
{
    public class EncryptedClaim : Claim
    {
        public EncryptedClaim(string type, string value) : base(type, value)
        {
        }
    }
}
