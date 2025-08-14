//  Authors:  Robert M. Scheller, James B. Domingo

using Landis.SpatialModeling;
using Landis.Library.UniversalCohorts;
using Landis.Core;
using System.Collections.Generic;
using Landis.Library.InitialCommunities.Universal;

namespace Landis.Extension.Succession.Biomass
{
    /// <summary>
    /// The initial live and dead biomass at a site.
    /// </summary>
    public class InitialBiomass
    {
        private SiteCohorts cohorts;
        private Library.UniversalCohorts.Pool deadWoodyPool;
        private Library.UniversalCohorts.Pool deadNonWoodyPool;
        
        //---------------------------------------------------------------------

        private static IDictionary<uint, InitialBiomass> initialSites;
        //  Initial site biomass for each unique pair of initial
        //  community and ecoregion; Key = 64-bit unsigned integer where
        //  high 64-bits is the map code of the initial community and the
        //  low 16-bits is the ecoregion's map code

        private static IDictionary<uint, List<ICohort>> sortedCohorts;
        //  Age cohorts for an initial community sorted from oldest to
        //  youngest.  Key = initial community's map code

        private static ushort successionTimestep;


        //---------------------------------------------------------------------

        /// <summary>
        /// The site's initial cohorts.
        /// </summary>
        public SiteCohorts Cohorts
        {
            get
            {
                return cohorts;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The site's initial dead woody pool.
        /// </summary>
        public Landis.Library.UniversalCohorts.Pool DeadWoodyPool
        {
            get
            {
                return deadWoodyPool;
            }
        }

        ////---------------------------------------------------------------------

        ///// <summary>
        ///// The site's initial dead non-woody pool.
        ///// </summary>
        public Landis.Library.UniversalCohorts.Pool DeadNonWoodyPool
        {
            get
            {
                return deadNonWoodyPool;
            }
        }

        //---------------------------------------------------------------------

        private InitialBiomass(SiteCohorts cohorts,
                               Landis.Library.UniversalCohorts.Pool deadWoodyPool,
                               Landis.Library.UniversalCohorts.Pool deadNonWoodyPool)
        {
            this.cohorts = cohorts;
            this.deadWoodyPool = deadWoodyPool;
            this.deadNonWoodyPool = deadNonWoodyPool;
        }

        //---------------------------------------------------------------------
        static InitialBiomass()
        {
            initialSites = new Dictionary<uint, InitialBiomass>();
            sortedCohorts = new Dictionary<uint, List<ICohort>>();
        }

        //---------------------------------------------------------------------
        /// Initializes this class.  The plug-in's timestep is used for growing biomass cohorts.
        public static void Initialize(int timestep)
        {
            successionTimestep = (ushort)timestep;
        }

        //private InitialBiomass(ISiteCohorts cohorts)
        //{
        //    this.cohorts = cohorts;
        //    //this.deadWoodyPool = deadWoodyPool;
        //    //this.deadNonWoodyPool = deadNonWoodyPool;
        //}

        //---------------------------------------------------------------------
        // This method is for NO SPIN UP - Values are input initial community CSV file
        public static InitialBiomass Compute(ActiveSite site, ICommunity initialCommunity)
        {
            IEcoregion ecoregion = PlugIn.ModelCore.Ecoregion[site];

            if (!ecoregion.Active)
            {
                string mesg = string.Format("Initial community {0} is located on a non-active ecoregion {1}", initialCommunity.MapCode, ecoregion.Name);
                throw new System.ApplicationException(mesg);
            }

            InitialBiomass initialBiomass;

            List<ICohort> sortedAgeCohorts = SortCohorts(initialCommunity.Cohorts);

            SiteCohorts cohorts = MakeBiomassCohorts(sortedAgeCohorts, site);
            initialBiomass = new InitialBiomass(cohorts, SiteVars.WoodyDebris[site],SiteVars.Litter[site]);

            return initialBiomass;
        }

        //---------------------------------------------------------------------
        // This method is for NO SPIN UP - Values are input initial community CSV file
        public static SiteCohorts MakeBiomassCohorts(List<ICohort> sortedCohorts, ActiveSite site)
        {

            IEcoregion ecoregion = PlugIn.ModelCore.Ecoregion[site];

            SiteVars.Cohorts[site] = new SiteCohorts();

            foreach (ICohort cohort in sortedCohorts)
            {
                SiteVars.Cohorts[site].AddNewCohort(cohort.Species, cohort.Data.Age, cohort.Data.Biomass, 0, cohort.Data.AdditionalParameters);
            }
            return SiteVars.Cohorts[site];
        }



        // *****************************************************
        // Functions Below are for Operating with Cohort Spin Up
        // *****************************************************

        public static List<ICohort> SortCohorts(List<ISpeciesCohorts> sppCohorts)
        {
            List<ICohort> cohorts = new List<ICohort>();
            foreach (ISpeciesCohorts speciesCohorts in sppCohorts)
            {
                foreach (ICohort cohort in speciesCohorts)
                {
                    cohorts.Add(cohort);
                    //PlugIn.ModelCore.UI.WriteLine("ADDED:  {0} {1}.", cohort.Species.Name, cohort.Age);
                }
            }
            cohorts.Sort(WhichIsOlderCohort);
            return cohorts;
        }

        private static int WhichIsOlderCohort(ICohort x, ICohort y)
        {
            return WhichIsOlder(x.Data.Age, y.Data.Age);
        }

        private static int WhichIsOlder(ushort x, ushort y)
        {
            return y - x;
        }

        public static SiteCohorts Clone(SiteCohorts site_cohorts)
        {
            SiteCohorts clone = new SiteCohorts();
            foreach (ISpeciesCohorts speciesCohorts in site_cohorts)
                foreach (ICohort cohort in speciesCohorts)
                    clone.AddNewCohort(cohort.Species, cohort.Data.Age, cohort.Data.Biomass, new System.Dynamic.ExpandoObject());  //species.cohorts.Add(speciesCohorts.Clone());
            return clone;
        }
        //---------------------------------------------------------------------

        private static uint ComputeKey(uint initCommunityMapCode,
                                       ushort ecoregionMapCode)
        {
            return (uint)((initCommunityMapCode << 16) | ecoregionMapCode);
        }


        //---------------------------------------------------------------------
        /// <summary>
        /// Computes the initial biomass at a site.
        /// </summary>
        public static InitialBiomass ComputeSpinUp(ActiveSite site, ICommunity initialCommunity)
        {
            IEcoregion ecoregion = PlugIn.ModelCore.Ecoregion[site];
            uint key = ComputeKey(initialCommunity.MapCode, ecoregion.MapCode);
            InitialBiomass initialBiomass;
            if (initialSites.TryGetValue(key, out initialBiomass))
                return initialBiomass;

            //  If we don't have a sorted list of age cohorts for the initial
            //  community, make the list
            List<ICohort> sortedAgeCohorts;
            if (!sortedCohorts.TryGetValue(initialCommunity.MapCode, out sortedAgeCohorts))
            {
                sortedAgeCohorts = SortCohorts(initialCommunity.Cohorts);
                sortedCohorts[initialCommunity.MapCode] = sortedAgeCohorts;
            }

            SiteCohorts cohorts = MakeBiomassCohortsSpinUp(sortedAgeCohorts, site);
            initialBiomass = new InitialBiomass(cohorts,
                                                SiteVars.WoodyDebris[site],
                                                SiteVars.Litter[site]);
            initialSites[key] = initialBiomass;
            return initialBiomass;
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Makes the set of biomass cohorts using cohort Spin Up
        public static SiteCohorts MakeBiomassCohortsSpinUp(List<ICohort> cohortsList, ActiveSite site)
        {
            return GrowCohorts(cohortsList, site, CohortBiomass.InitialBiomass);
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Makes the set of biomass cohorts at a site, using input Biomass values
        /// </summary>
        public static SiteCohorts GrowCohorts(List<ICohort> cohortList, ActiveSite site, ComputeMethod initialBiomassMethod)
        {

            SiteVars.Cohorts[site] = new SiteCohorts();

            if (cohortList.Count == 0)
                return SiteVars.Cohorts[site];

            int indexNextCohort = 0;
            //  The index in the list of sorted cohorts of the next cohort to be considered

            //  Loop through time from -N to 0 where N is the oldest cohort.
            //  So we're going from the time when the oldest cohort was "born"
            //  to the present time (= 0).  Because the age of any cohort
            //  is a multiple of the succession timestep, we go from -N to 0
            //  by that timestep.  NOTE: the case where timestep = 1 requires
            //  special treatment because if we start at time = -N with a
            //  cohort with age = 1, then at time = 0, its age will N+1 not N.
            //  Therefore, when timestep = 1, the ending time is -1.
            int endTime = (successionTimestep == 1) ? -1 : 0;
            for (int time = -(cohortList[0].Data.Age); time <= endTime; time += successionTimestep)
            {
                //  Grow current biomass cohorts.
                PlugIn.GrowCohorts(site, successionTimestep, true);

                //  Add those cohorts that were born at the current year
                while (indexNextCohort < cohortList.Count &&
                       cohortList[indexNextCohort].Data.Age == -time)
                {

                    ISpecies species = cohortList[indexNextCohort].Species;

                    int initialBiomass = initialBiomassMethod(species, SiteVars.Cohorts[site], site);

                    SiteVars.Cohorts[site].AddNewCohort(cohortList[indexNextCohort].Species, 1,
                                                initialBiomass, new System.Dynamic.ExpandoObject());
                    indexNextCohort++;
                }
            }

            //PlugIn.ModelCore.Log.WriteLine("Initial Community = {0}.", SiteVars.Cohorts[site].Write());
            return SiteVars.Cohorts[site];
        }



        //---------------------------------------------------------------------
        // Makes a list of cohorts in an initial community sorted from oldest to youngest.
        public static List<ICohort> SortCohortsSpinUp(List<ISpeciesCohorts> sppCohorts)
        {
            List<ICohort> cohorts = new List<ICohort>();
            foreach (ISpeciesCohorts speciesCohorts in sppCohorts)
            {
                foreach (ICohort cohort in speciesCohorts)
                    cohorts.Add(cohort);
            }
            cohorts.Sort(Landis.Library.UniversalCohorts.Util.WhichIsOlderCohort);
            return cohorts;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// A method that computes the initial biomass for a new cohort at a
        /// site based on the existing cohorts.
        /// </summary>
        public delegate int ComputeMethod(ISpecies species, SiteCohorts SiteCohorts, ActiveSite site);


    }
}
