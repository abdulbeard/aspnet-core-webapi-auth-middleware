using System.Security.Claims;
using System.Threading.Tasks;

namespace MisturTee.Config.Claims.ExtractionConfigs.Valid
{
    /// <summary>
    /// Represents a ClaimsExtractionConfig that is configured correctly
    /// </summary>
    public interface IValidClaimsExtractionConfig
    {
        /// <summary>
        /// Defines what method to use to extract the claim
        /// </summary>
        ExtractionType ExtractionType { get; }
        /// <summary>
        /// The location in the request where the claim can be found
        /// </summary>
        ClaimLocation ClaimLocation { get; }
        /// <summary>
        /// This function uses the <see cref="ExtractionType"/> and <see cref="ClaimLocation"/> to extract the claim from the content
        /// </summary>
        /// <param name="content">string representation of the content that contains the claim info</param>
        /// <returns></returns>
        Task<Claim> GetClaimAsync(string content);
    }
}
