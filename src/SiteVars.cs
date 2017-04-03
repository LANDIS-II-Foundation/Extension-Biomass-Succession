//  Copyright 2005-2010 Portland State University, University of Wisconsin
//  Authors:  Robert M. Scheller, James B. Domingo

using Landis.SpatialModeling;
using Landis.Library.BiomassCohorts;

namespace Landis.Extension.Succession.Biomass
{
    /// <summary>
    /// Site Variables for a landscape.
    /// </summary>
    public static class SiteVars
    {

        private static ISiteVar<Landis.Library.BiomassCohorts.SiteCohorts> biomassCohorts;
        
        private static ISiteVar<Landis.Library.AgeOnlyCohorts.ISiteCohorts> baseCohortsSiteVar;

        private static ISiteVar<Landis.Library.Biomass.Pool> woodyDebris;
        private static ISiteVar<Landis.Library.Biomass.Pool> litter;
        
        private static ISiteVar<double> capacityReduction;
        private static ISiteVar<int> previousYearMortality;
        private static ISiteVar<int> currentYearMortality;
        private static ISiteVar<int> totalBiomass;

        private static ISiteVar<double> ag_npp;
        public static ISiteVar<double> Defoliation;


        //---------------------------------------------------------------------
        /// <summary>
        /// Initializes the module.
        /// </summary>
        public static void Initialize()
        {
            biomassCohorts = PlugIn.ModelCore.Landscape.NewSiteVar<Library.BiomassCohorts.SiteCohorts>();
            ISiteVar<Landis.Library.BiomassCohorts.ISiteCohorts> biomassCohortSiteVar = Landis.Library.Succession.CohortSiteVar<Landis.Library.BiomassCohorts.ISiteCohorts>.Wrap(biomassCohorts);
            
            baseCohortsSiteVar = Landis.Library.Succession.CohortSiteVar<Landis.Library.AgeOnlyCohorts.ISiteCohorts>.Wrap(biomassCohorts);
            

            woodyDebris = PlugIn.ModelCore.Landscape.NewSiteVar<Landis.Library.Biomass.Pool>();
            litter = PlugIn.ModelCore.Landscape.NewSiteVar<Landis.Library.Biomass.Pool>();
            ag_npp          = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            Defoliation     = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            previousYearMortality = PlugIn.ModelCore.Landscape.NewSiteVar<int>();
            currentYearMortality = PlugIn.ModelCore.Landscape.NewSiteVar<int>();
            totalBiomass = PlugIn.ModelCore.Landscape.NewSiteVar<int>();

            foreach (ActiveSite site in PlugIn.ModelCore.Landscape)
            {
                //  site cohorts are initialized by the PlugIn.InitializeSite method
                woodyDebris[site] = new Landis.Library.Biomass.Pool();
                litter[site] = new Landis.Library.Biomass.Pool();
            }

            currentYearMortality.ActiveSiteValues = 0;
            previousYearMortality.ActiveSiteValues = 0;

            PlugIn.ModelCore.RegisterSiteVar(biomassCohortSiteVar, "Succession.BiomassCohorts");
            PlugIn.ModelCore.RegisterSiteVar(baseCohortsSiteVar, "Succession.AgeCohorts");
 

            PlugIn.ModelCore.RegisterSiteVar(SiteVars.WoodyDebris, "Succession.WoodyDebris");
            PlugIn.ModelCore.RegisterSiteVar(SiteVars.Litter, "Succession.Litter");

        }

        //---------------------------------------------------------------------
        public static void ResetAnnualValues(Site site)
        {

            // Reset these accumulators to zero:
            SiteVars.AGNPP[site] = 0.0;
            SiteVars.Defoliation[site] = 0.0;
            SiteVars.TotalBiomass[site] = 0;
            SiteVars.TotalBiomass[site] = Library.BiomassCohorts.Cohorts.ComputeNonYoungBiomass(SiteVars.Cohorts[site]);

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
                return biomassCohorts;
            }
            set
            {
                biomassCohorts = value;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The intact dead woody pools for the landscape's sites.
        /// </summary>
        public static ISiteVar<Landis.Library.Biomass.Pool> WoodyDebris
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
        public static ISiteVar<Landis.Library.Biomass.Pool> Litter
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
