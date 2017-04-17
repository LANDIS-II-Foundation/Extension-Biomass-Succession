//  Copyright 2005-2010 Portland State University, University of Wisconsin
//  Authors:  Robert M. Scheller, James B. Domingo

using Landis.SpatialModeling;
using Landis.Core;
using Landis.Library.BiomassCohorts;
using System.Collections.Generic;
using Landis.Utilities;

namespace Landis.Extension.Succession.Biomass.AgeOnlyDisturbances
{
    /// <summary>
    /// The handlers for various type of events related to age-only
    /// disturbances.
    /// </summary>
    public static class Events
    {
        public static void CohortDied(object         sender,
                                      DeathEventArgs eventArgs)
        {
            ExtensionType disturbanceType = eventArgs.DisturbanceType;
            PoolPercentages cohortReductions = Module.Parameters.CohortReductions[disturbanceType];

            ICohort cohort = (Landis.Library.BiomassCohorts.ICohort) eventArgs.Cohort;
            ActiveSite site = eventArgs.Site;
            int nonWoody = cohort.ComputeNonWoodyBiomass(site);
            int woody = (cohort.Biomass - nonWoody);

            int nonWoodyInput = ReduceInput(nonWoody, cohortReductions.Foliar);
            int woodyInput = ReduceInput(woody, cohortReductions.Wood);

            //ForestFloor.AddBiomass(woodyInput, nonWoodyInput, cohort.Species, site);
            ForestFloor.AddWoody(woodyInput, cohort.Species, site);
            ForestFloor.AddLitter(nonWoodyInput, cohort.Species, site);
        }

        //---------------------------------------------------------------------

        public static int ReduceInput(int     poolInput,
                                          Percentage reductionPercentage)
        {
            int reduction = (int) (poolInput * reductionPercentage);
            return (int) (poolInput - reduction);
        }

        //---------------------------------------------------------------------

        public static void SiteDisturbed(object               sender,
                                         DisturbanceEventArgs eventArgs)
        {
            ExtensionType disturbanceType = eventArgs.DisturbanceType;
            PoolPercentages poolReductions = Module.Parameters.PoolReductions[disturbanceType];

            ActiveSite site = eventArgs.Site;
            SiteVars.WoodyDebris[site].ReduceMass(poolReductions.Wood);
            SiteVars.Litter[site].ReduceMass(poolReductions.Foliar);
        }
    }
}
