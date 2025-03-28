//  Authors:  Robert M. Scheller, James B. Domingo

using Landis.SpatialModeling;
using Landis.Library.UniversalCohorts;

namespace Landis.Extension.Succession.Biomass
{
    /// <summary>
    /// Site Variables for a landscape.
    /// </summary>
    public static class SiteVars
    {
        public static ISiteVar<double> Defoliation;
        public static ISiteVar<string> HarvestPrescriptionName;
        public static ISiteVar<byte> FireSeverity;
        public static ISiteVar<int> MaxBiomass;



        //---------------------------------------------------------------------
        /// <summary>
        /// Initializes the module.
        /// </summary>
        public static void Initialize()
        {
            cohorts = PlugIn.ModelCore.Landscape.NewSiteVar<SiteCohorts>();
            //Cohorts = Landis.Library.Succession.CohortSiteVar<ISiteCohorts>.Wrap(Cohorts);

            HarvestPrescriptionName = PlugIn.ModelCore.GetSiteVar<string>("Harvest.PrescriptionName");

            WoodyDebris = PlugIn.ModelCore.Landscape.NewSiteVar<Landis.Library.UniversalCohorts.Pool>();
            Litter = PlugIn.ModelCore.Landscape.NewSiteVar<Landis.Library.UniversalCohorts.Pool>();
            AGNPP          = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            Defoliation     = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            PreviousYearMortality = PlugIn.ModelCore.Landscape.NewSiteVar<int>();
            CurrentYearMortality = PlugIn.ModelCore.Landscape.NewSiteVar<int>();
            TotalBiomass = PlugIn.ModelCore.Landscape.NewSiteVar<int>();
            MaxBiomass = PlugIn.ModelCore.Landscape.NewSiteVar<int>();
            FireSeverity = PlugIn.ModelCore.GetSiteVar<byte>("Fire.Severity");

            foreach (ActiveSite site in PlugIn.ModelCore.Landscape)
            {
                //  site cohorts are initialized by the PlugIn.InitializeSite method
                WoodyDebris[site] = new Landis.Library.UniversalCohorts.Pool();
                Litter[site] = new Landis.Library.UniversalCohorts.Pool();
            }

            CurrentYearMortality.ActiveSiteValues = 0;
            PreviousYearMortality.ActiveSiteValues = 0;

            PlugIn.ModelCore.RegisterSiteVar(cohorts, "Succession.UniversalCohorts");
            PlugIn.ModelCore.RegisterSiteVar(SiteVars.WoodyDebris, "Succession.WoodyDebris");
            PlugIn.ModelCore.RegisterSiteVar(SiteVars.Litter, "Succession.Litter");
            PlugIn.ModelCore.RegisterSiteVar(SiteVars.MaxBiomass, "Succession.MaxBiomass");
        }

        //---------------------------------------------------------------------
        public static void ResetAnnualValues(Site site)
        {

            // Reset these accumulators to zero:
            SiteVars.AGNPP[site] = 0.0;
            SiteVars.Defoliation[site] = 0.0;
            SiteVars.TotalBiomass[site] = 0;
            SiteVars.TotalBiomass[site] = Landis.Library.UniversalCohorts.Cohorts.ComputeNonYoungBiomass(SiteVars.Cohorts[site]);

            SiteVars.PreviousYearMortality[site] = SiteVars.CurrentYearMortality[site];
            SiteVars.CurrentYearMortality[site] = 0;


        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Cohorts at each site.
        /// </summary>
        private static ISiteVar<SiteCohorts> cohorts;
        public static ISiteVar<SiteCohorts> Cohorts
        {
            get
            {
                return cohorts;
            }
            set
            {
                cohorts = value;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The intact dead woody pools for the landscape's sites.
        /// </summary>
        public static ISiteVar<Landis.Library.UniversalCohorts.Pool> WoodyDebris { get; private set; }

        //---------------------------------------------------------------------

        /// <summary>
        /// The dead non-woody pools for the landscape's sites.
        /// </summary>
        public static ISiteVar<Landis.Library.UniversalCohorts.Pool> Litter { get; private set; }


        //---------------------------------------------------------------------

        /// <summary>
        /// Previous Year Site Mortality.
        /// </summary>
        public static ISiteVar<int> PreviousYearMortality { get; private set; }

        //---------------------------------------------------------------------

        /// <summary>
        /// Previous Year Site Mortality.
        /// </summary>
        public static ISiteVar<int> CurrentYearMortality { get; private set; }
        //---------------------------------------------------------------------

        /// <summary>
        /// Previous Year Site Mortality.
        /// </summary>
        public static ISiteVar<int> TotalBiomass { get; private set; }
        //---------------------------------------------------------------------
        /// <summary>
        /// </summary>
        public static ISiteVar<double> HarvestCapacityReduction { get; set; }
        //---------------------------------------------------------------------

        /// <summary>
        /// </summary>
        public static ISiteVar<double> AGNPP { get; set; }
    }
}
