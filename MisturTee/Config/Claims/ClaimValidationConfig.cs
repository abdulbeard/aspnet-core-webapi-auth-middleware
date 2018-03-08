using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MisturTee.Config.Claims
{
    public class ClaimValidationConfig
    {
        public bool IsRequired { get; set; }
        public string ClaimName { get; set; }
        public bool AllowNullOrEmpty { get; set; }
        public bool ValueMustBeExactMatch { get; set; }

        /// <summary>
        /// Delegate that takes in <see cref="HttpContext"/> and evaluates if all is good with the request
        /// </summary>
        /// <param name="context">current http context</param>
        /// <returns>
        /// true if claims are valid and all is good with the request. 
        /// false if errors were found with the request, and the request will be cut short and the response returned
        /// </returns>
        public delegate Task<bool> ClaimValidatorDelegate(HttpContext context);
    }
}
