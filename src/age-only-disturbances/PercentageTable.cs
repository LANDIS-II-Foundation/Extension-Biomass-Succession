//  Copyright 2005-2010 Portland State University, University of Wisconsin
//  Authors:  Robert M. Scheller, James B. Domingo

using Landis.Core;
using System.Collections.Generic;

namespace Landis.Extension.Succession.Biomass.AgeOnlyDisturbances
{
    /// <summary>
    /// A table of pool percentages for age-only disturbances.
    /// </summary>
    public interface IPercentageTable
    {
        /// <summary>
        /// Gets the pair of percentages for a particular disturbance type.
        /// </summary>
        PoolPercentages this[ExtensionType disturbanceType]
        {
            get;
        }
        
        PoolPercentages Default {get;}
    }

    /// <summary>
    /// A table of pool percentages for age-only disturbances.
    /// </summary>
    public class PercentageTable
        : IPercentageTable
    {
        private IDictionary<ExtensionType, PoolPercentages> percentages;
        private PoolPercentages defaultPercentages;

        //---------------------------------------------------------------------

        /// <summary>
        /// Gets the pair of percentages for a particular disturbance type.
        /// </summary>
        public PoolPercentages this[ExtensionType disturbanceType]
        {
            get {
                PoolPercentages poolPercentages;
                if (percentages.TryGetValue(disturbanceType, out poolPercentages))
                    return poolPercentages;

                return defaultPercentages;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The default pair of percentages for disturbance types that do not
        /// have an entry in the table.
        /// </summary>
        public PoolPercentages Default
        {
            get {
                return defaultPercentages;
            }
        }
        //---------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
/*        public PercentageTable(IDictionary<ExtensionType, PoolPercentages> percentages,
                               PoolPercentages                          defaultPercentages)
        {
            this.percentages = percentages;
            this.defaultPercentages = defaultPercentages;
        }*/
        //---------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        public PercentageTable()
        {
            this.percentages = new Dictionary<ExtensionType, PoolPercentages>();
            this.defaultPercentages = new PoolPercentages();
        }

    }
}
