using Landis.Succession;
using System.Collections.Generic;

namespace Landis.Biomass.Succession
{
    /// <summary>
    /// The parameters for biomass succession.
    /// </summary>
    public interface IParameters
        : ClimateChange.IParameters
    {
        /// <summary>
        /// Timestep (years)
        /// </summary>
        int Timestep
        {
            get;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Seeding algorithm
        /// </summary>
        SeedingAlgorithms SeedAlgorithm
        {
            get;
        }

        bool CalibrateMode {get; set;}
        double SpinupMortalityFraction {get; set;}

        //---------------------------------------------------------------------

        /// <summary>
        /// Path to the optional file with the biomass parameters for age-only
        /// disturbances.
        /// </summary>
        string AgeOnlyDisturbanceParms
        {
            get;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// A list of zero or more updates to the biomass parameters because of
        /// climate change.
        /// </summary>
        List<ClimateChange.ParametersUpdate> ClimateChangeUpdates
        {
            get;
        }
    }
}
