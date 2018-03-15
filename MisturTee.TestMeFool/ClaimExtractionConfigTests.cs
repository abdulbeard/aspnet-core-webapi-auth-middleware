using MisturTee.Config.Claims.ExtractionConfigs;
using Xunit;

namespace MisturTee.TestMeFool
{
    public class ClaimExtractionConfigTests
    {
        public void JsonPathClaimExtractionConfig()
        {
            var jsonPathClaimExtractionConfig = new JsonPathClaimExtractionConfig("PityTheFoolClaim");
            Assert.Equal(1,1);
        }
    }
}
