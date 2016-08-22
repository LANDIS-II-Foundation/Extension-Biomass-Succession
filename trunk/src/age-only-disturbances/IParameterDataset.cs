//  Copyright 2005-2010 Portland State University, University of Wisconsin
//  Authors:  Robert M. Scheller, James B. Domingo

using Landis.Core;

namespace Landis.Extension.Succession.Biomass.AgeOnlyDisturbances
{
    /// <summary>
    /// A collection of biomass parameters for age-only disturbances.
    /// </summary>
    public interface IParameterDataset
    {
        /// <summary>
        /// A table of percentages that a disturbance reduce a cohort's
        /// inputs to the dead pools.
        /// </summary>
        IPercentageTable CohortReductions
        {
            get;set;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// A table of percentages that a disturbances reduces a site's dead
        /// pools.
        /// </summary>
        IPercentageTable PoolReductions
        {
            get;set;
        }
    }
}
