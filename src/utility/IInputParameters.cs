//  Authors:  Robert M. Scheller, James B. Domingo

using Landis.Library.Succession;
using Landis.Core;

using Landis.Utilities;

using System.Collections.Generic;
using System.Diagnostics;

namespace Landis.Extension.Succession.Biomass
{
    /// <summary>
    /// The parameters for biomass succession.
    /// </summary>
    public interface IInputParameters
    {
        int Timestep {get; set;}
        SeedingAlgorithms SeedAlgorithm {get; set;}
        string InitialCommunities{get; set;}
        string InitialCommunitiesMap{get; set;}
        string WoodyDebrisMap { get; set; }
        string LitterMap { get; set; }
        string ClimateConfigFile { get; set; }
        bool CalibrateMode { get; set; }
        double SpinupMortalityFraction {get; set;}
        List<ISufficientLight> LightClassProbabilities {get; set;}
        Landis.Library.Parameters.Ecoregions.AuxParm<Percentage>[] MinRelativeBiomass { get; }
        Landis.Library.Parameters.Species.AuxParm<double> LeafLongevity { get; }
        Landis.Library.Parameters.Species.AuxParm<double> WoodyDecayRate { get; }
        Landis.Library.Parameters.Species.AuxParm<double> MortCurveShapeParm { get; }
        Landis.Library.Parameters.Species.AuxParm<double> GrowthCurveShapeParm { get; }
        Landis.Library.Parameters.Species.AuxParm<double> LeafLignin { get; }

        Landis.Library.Parameters.Ecoregions.AuxParm<int> AET { get; }
        string DynamicInputFile {get;set;}

        //---------------------------------------------------------------------
        /// <summary>
        /// Parameters for fire effects on wood and leaf litter
        /// </summary>
        FireReductions[] FireReductionsTable
        {
            get; set;
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Parameters for harvest or fuel treatment effects on wood and leaf litter
        /// </summary>
        List<HarvestReductions> HarvestReductionsTable
        {
            get;
            set;
        }
    }


}
