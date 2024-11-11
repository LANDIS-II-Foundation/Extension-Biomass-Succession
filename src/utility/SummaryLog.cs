using Landis.Library.Metadata;

namespace Landis.Extension.Succession.Biomass
{
    public class SummaryLog
    {
        [DataFieldAttribute(Unit = FieldUnits.Year, Desc = "Time")]
        public int Time { get; set; }

        [DataFieldAttribute(Desc = "Ecoregoin Name")]
        public string EcoName { get; set; }

        [DataFieldAttribute(Unit = FieldUnits.Count, Desc = "Active Site Count")]
        public int ActiveCount { get; set; }

        [DataFieldAttribute(Unit = FieldUnits.g_B_m2, Desc = "Average LiveB", Format = "0.0")]
        public double AvgLiveB { get; set; }

        [DataFieldAttribute(Unit = FieldUnits.g_B_m2_yr1, Desc = "Average AG_NPP", Format = "0")]
        public int AvgAG_NPP { get; set; }

        [DataFieldAttribute(Unit = FieldUnits.g_B_m2, Desc = "Average LitterB", Format = "0.0")]
        public double AvgLitterB { get; set; }

        [DataFieldAttribute(Unit = FieldUnits.g_B_m2, Desc = "Average WoodLitterB", Format = "0.0")]
        public double AvgWoodLitterB { get; set; }

        [DataFieldAttribute(Unit = FieldUnits.g_B_m2, Desc = "Average Defoliation", Format = "0.0")]
        public double AvgDefoliation { get; set; }

    }
}