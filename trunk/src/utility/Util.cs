//  Copyright 2005-2010 Portland State University, University of Wisconsin
//  Authors:  Robert M. Scheller

using Landis.SpatialModeling;
using Landis.Core;
using Edu.Wisc.Forest.Flel.Util;

namespace Landis.Extension.Succession.Biomass
{
    /// <summary>
    /// Utility methods.
    /// </summary>
    public static class Util
    {

        //---------------------------------------------------------------------

        public static Species.AuxParm<Ecoregions.AuxParm<T>> CreateSpeciesEcoregionParm<T>(ISpeciesDataset speciesDataset, IEcoregionDataset ecoregionDataset)
        {
            Species.AuxParm<Ecoregions.AuxParm<T>> newParm;
            newParm = new Species.AuxParm<Ecoregions.AuxParm<T>>(speciesDataset);
            foreach (ISpecies species in speciesDataset) {
                newParm[species] = new Ecoregions.AuxParm<T>(ecoregionDataset);
            }
            return newParm;
        }
        //---------------------------------------------------------------------

        public static double CheckBiomassParm(InputValue<double> newValue,
                                                    double             minValue,
                                                    double             maxValue)
        {
            if (newValue != null) {
                if (newValue.Actual < minValue || newValue.Actual > maxValue)
                    throw new InputValueException(newValue.String,
                                                  "{0} is not between {1:0.0} and {2:0.0}",
                                                  newValue.String, minValue, maxValue);
            }
            return newValue.Actual;
        }
        //---------------------------------------------------------------------

        public static int CheckBiomassParm(InputValue<int> newValue,
                                                    int             minValue,
                                                    int             maxValue)
        {
            if (newValue != null) {
                if (newValue.Actual < minValue || newValue.Actual > maxValue)
                    throw new InputValueException(newValue.String,
                                                  "{0} is not between {1:0.0} and {2:0.0}",
                                                  newValue.String, minValue, maxValue);
            }
            return newValue.Actual;
        }
        //---------------------------------------------------------------------

        /// <summary>
        /// Converts a table indexed by species and ecoregion into a
        /// 2-dimensional array.
        /// </summary>
        public static T[,] ToArray<T>(Species.AuxParm<Ecoregions.AuxParm<T>> table)
        {
            T[,] array = new T[PlugIn.ModelCore.Ecoregions.Count, PlugIn.ModelCore.Species.Count];
            foreach (ISpecies species in PlugIn.ModelCore.Species) {
                foreach (IEcoregion ecoregion in PlugIn.ModelCore.Ecoregions) {
                    array[ecoregion.Index, species.Index] = table[species][ecoregion];
                }
            }
            return array;
        }
    }
}
