namespace MiddlewareAuth.Config.Claims
{
    public class ClaimValidationConfig
    {
        public bool IsRequired { get; set; }
        public string ClaimName { get; set; }
        public bool AllowNullOrEmpty { get; set; }
        public bool ValueMustBeExactMatch { get; set; }
    }
}
