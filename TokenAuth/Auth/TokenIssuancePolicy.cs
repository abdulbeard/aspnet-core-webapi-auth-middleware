using System;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace TokenAuth.Auth
{
    public class TokenIssuancePolicy
    {
        public static TokenIssuancePolicy Default = new TokenIssuancePolicy();
        private const string Key = "C5AxWRAoC/lp3Ayt1RcAxMQDZ74fy1f6rzA7ko1GME06/FkBhRML1BNLXMwTVeoRAJ2oVvIdTy8b4Px8FgJ7e36hCp6SopZhoAng1HwPtLYg4QUXMfjCjaKEqba4/e5nsZXaJpn9a6CaSFy6WL3PPV5m7ZyFK+jLlhT+X5inqPk=";
        public TokenIssuancePolicy()
        {
            Expiration = TimeSpan.FromHours(8);
            Issuer = AppDomain.CurrentDomain.FriendlyName;
            NotBefore = TimeSpan.Zero;
            TokenId = Guid.NewGuid();
            SecurityKey = new SymmetricSecurityKey(Encoding.Default.GetBytes(Key));
        }

        public TimeSpan Expiration { get; set; }
        public string Issuer { get; set; }
        public TimeSpan NotBefore { get; set; }
        public Guid TokenId { get; set; }
        public SymmetricSecurityKey SecurityKey { get; set; }
    }
}
