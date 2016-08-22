//  Copyright 2005-2010 Portland State University, University of Wisconsin
//  Authors:  Robert M. Scheller, James B. Domingo

using Landis.Core;

namespace Landis.Extension.Succession.Biomass.Ecoregions
{
    /// <summary>
    /// An auxiliary parameter for ecoregions.
    /// </summary>
    public class AuxParm<T>
    {
        private T[] values;

        //---------------------------------------------------------------------

        public T this[IEcoregion ecoregion]
        {
            get
            {
                return values[ecoregion.Index];
            }

            set
            {
                values[ecoregion.Index] = value;
            }
        }

        //---------------------------------------------------------------------

        public AuxParm(IEcoregionDataset ecoregions)
        {
            values = new T[ecoregions.Count];
        }
    }
}
