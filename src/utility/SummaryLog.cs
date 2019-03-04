using Landis.Library.Metadata;

namespace Landis.Extension.Succession.Biomass
{
    public class SummaryLog
    {
        [DataFieldAttribute(Unit = FieldUnits.Year, Desc = "Time")]
        public int Time { get; set; }

        [DataFieldAttribute(Desc = "Ecoregoin Name")]
        public string EcoName { get; set; }

        [DataFieldAttribute(Desc = "Active Site Count")]
        public int ActiveCount { get; set; }

        [DataFieldAttribute(Desc = "Average LiveB")]
        public double AvgLiveB { get; set; }

        [DataFieldAttribute(Desc = "Average AG_NPP")]
        public double AvgAG_NPP { get; set; }

        [DataFieldAttribute(Desc = "Average LitterB")]
        public double AvgLitterB { get; set; }

        [DataFieldAttribute(Desc = "Average WoodLitterB")]
        public double AvgWoodLitterB { get; set; }

        [DataFieldAttribute(Desc = "Average Defoliation")]
        public double AvgDefoliation { get; set; }

    }
}