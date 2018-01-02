using System;

namespace TokenAuth.Models
{
    public class MoralValues
    {
        public Guid MoralValueId { get; set; }
        public Alignment Alignment { get; set; }
        public AltruisticAptitude AltruisticAptitude { get; set; }
        public string ReportViolationEmail { get; set; }
    }

    public class AltruisticAptitude
    {
        public bool IsTrueAltruism { get; set; }
        public AltruisticAlignment AltruisticAlignment { get; set; }
    }

    public enum AltruisticAlignment
    {
        Agnostic,
        Atheistic,
        Theistic,
        TheisticUnspecified,
        ChristianNondenominational,
        Catholic,
        Christian,
        Muslim,
        Jewish
    }

    public enum Alignment
    {
        LawfulGood,
        NeutralGood,
        ChaoticGood,
        LawfulNeutral,
        Neutral,
        ChaoticNeutral,
        LawfulEvil,
        NeutralEvil,
        ChaoticEvil
    }
}


