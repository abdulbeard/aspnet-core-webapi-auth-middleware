using System.Security.Claims;
using System.Threading.Tasks;

namespace MisturTee.Config.Claims.ExtractionConfigs.Valid
{

    public interface IValidClaimsExtractionConfig
    {
        ExtractionType ExtractionType { get; }
        ClaimLocation ClaimLocation { get; }
        Task<Claim> GetClaimAsync(string content);
    }
}
