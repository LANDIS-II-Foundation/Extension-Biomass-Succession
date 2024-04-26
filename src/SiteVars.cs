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

        private static ISiteVar<SiteCohorts> universalCohorts;
        
        private static ISiteVar<ISiteCohorts> universalCohortsSiteVar;

        private static ISiteVar<Landis.Library.UniversalCohorts.Pool> woodyDebris;
        private static ISiteVar<Landis.Library.UniversalCohorts.Pool> litter;
        
        private static ISiteVar<double> capacityReduction;
        private static ISiteVar<int> previousYearMortality;
        private static ISiteVar<int> currentYearMortality;
        private static ISiteVar<int> totalBiomass;

        private static ISiteVar<double> ag_npp;
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
            universalCohorts = PlugIn.ModelCore.Landscape.NewSiteVar<SiteCohorts>();
            universalCohortsSiteVar = Landis.Library.Succession.CohortSiteVar<ISiteCohorts>.Wrap(universalCohorts);

            HarvestPrescriptionName = PlugIn.ModelCore.GetSiteVar<string>("Harvest.PrescriptionName");

            woodyDebris = PlugIn.ModelCore.Landscape.NewSiteVar<Landis.Library.UniversalCohorts.Pool>();
            litter = PlugIn.ModelCore.Landscape.NewSiteVar<Landis.Library.UniversalCohorts.Pool>();
            ag_npp          = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            Defoliation     = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            previousYearMortality = PlugIn.ModelCore.Landscape.NewSiteVar<int>();
            currentYearMortality = PlugIn.ModelCore.Landscape.NewSiteVar<int>();
            totalBiomass = PlugIn.ModelCore.Landscape.NewSiteVar<int>();
            MaxBiomass = PlugIn.ModelCore.Landscape.NewSiteVar<int>();
            FireSeverity = PlugIn.ModelCore.GetSiteVar<byte>("Fire.Severity");

            foreach (ActiveSite site in PlugIn.ModelCore.Landscape)
            {
                //  site cohorts are initialized by the PlugIn.InitializeSite method
                woodyDebris[site] = new Landis.Library.UniversalCohorts.Pool();
                litter[site] = new Landis.Library.UniversalCohorts.Pool();
            }

            currentYearMortality.ActiveSiteValues = 0;
            previousYearMortality.ActiveSiteValues = 0;

            PlugIn.ModelCore.RegisterSiteVar(universalCohortsSiteVar, "Succession.UniversalCohorts");
 

            PlugIn.ModelCore.RegisterSiteVar(SiteVars.WoodyDebris, "Succession.WoodyDebris");
            PlugIn.ModelCore.RegisterSiteVar(SiteVars.Litter, "Succession.Litter");
            PlugIn.ModelCore.RegisterSiteVar(SiteVars.MaxBiomass, "Succession.MaxBiomass");

            //EcoregionData.UpdateB_MAX();

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
        /// Biomass cohorts at each site.
        /// </summary>
        public static ISiteVar<SiteCohorts> Cohorts
        {
            get
            {
                return universalCohorts;
            }
            set
            {
                universalCohorts = value;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The intact dead woody pools for the landscape's sites.
        /// </summary>
        public static ISiteVar<Landis.Library.UniversalCohorts.Pool> WoodyDebris
        {
            get
            {
                return woodyDebris;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The dead non-woody pools for the landscape's sites.
        /// </summary>
        public static ISiteVar<Landis.Library.UniversalCohorts.Pool> Litter
        {
            get
            {
                return litter;
            }
        }


        //---------------------------------------------------------------------
        
        /// <summary>
        /// Previous Year Site Mortality.
        /// </summary>
        public static ISiteVar<int> PreviousYearMortality
        {
            get {
                return previousYearMortality;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Previous Year Site Mortality.
        /// </summary>
        public static ISiteVar<int> CurrentYearMortality
        {
            get
            {
                return currentYearMortality;
            }
        }
        //---------------------------------------------------------------------

        /// <summary>
        /// Previous Year Site Mortality.
        /// </summary>
        public static ISiteVar<int> TotalBiomass
        {
            get
            {
                return totalBiomass;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// </summary>
        public static ISiteVar<double> CapacityReduction
        {
            get {
                return capacityReduction;
            }
            set {
                capacityReduction = value;
            }
        }
        //---------------------------------------------------------------------

        /// <summary>
        /// </summary>
        public static ISiteVar<double> AGNPP
        {
            get {
                return ag_npp;
            }
            set {
                ag_npp = value;
            }
        }
    }
}
