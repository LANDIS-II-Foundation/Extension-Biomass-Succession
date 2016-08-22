//  Copyright 2006 University of Wisconsin-Madison
//  Author: Jimm Domingo, FLEL

using Landis.PlugIns;

namespace Landis.Biomass.Succession.AgeOnlyDisturbances
{
    /// <summary>
    /// A table of pool percentages for age-only disturbances.
    /// </summary>
    public interface IPercentageTable
    {
        /// <summary>
        /// Gets the pair of percentages for a particular disturbance type.
        /// </summary>
        PoolPercentages this[PlugInType disturbanceType]
        {
            get;
        }
    }
}
