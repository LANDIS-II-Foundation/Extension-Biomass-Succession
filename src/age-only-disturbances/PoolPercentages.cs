//  Copyright 2005-2010 Portland State University, University of Wisconsin
//  Authors:  Robert M. Scheller

using Landis.Utilities;

namespace Landis.Extension.Succession.Biomass.AgeOnlyDisturbances
{
    /// <summary>
    /// A pair of percentage parameters for the two dead pools.
    /// </summary>
    public class PoolPercentages
    {
        private Percentage wood;
        private Percentage foliar;

        //---------------------------------------------------------------------

        /// <summary>
        /// The percentage associated with the dead wood pool.
        /// </summary>
        public Percentage Wood
        {
            get {
                return wood;
            }
            set {
                ValidatePercentage(value);
                wood = value;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The percentage associated with the dead non-wood pool.
        /// pool.
        /// </summary>
        public Percentage Foliar
        {
            get {
                return foliar;
            }
            set {
                ValidatePercentage(value);
                foliar = value;
            }
        }

        //---------------------------------------------------------------------

        private void ValidatePercentage(Percentage percentage)
        {
            if (percentage < 0.0 || percentage > 1.0)
                throw new InputValueException(percentage.ToString(),
                                              "Value must be between 0% and 100%");
        }
        //---------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
/*        public PoolPercentages(Percentage wood,
                               Percentage foliar)
        {
            this.wood = wood;
            this.foliar = foliar;
        }*/
        //---------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        public PoolPercentages()
        {
        }
    }
}
