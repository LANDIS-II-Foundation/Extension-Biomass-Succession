//  Copyright 2005-2010 Portland State University, University of Wisconsin
//  Authors:  Robert M. Scheller, James B. Domingo

using Landis.SpatialModeling;
using Landis.Core;
using Edu.Wisc.Forest.Flel.Util;

namespace Landis.Extension.Succession.Biomass
{
    public class EcoregionData
    {

        //user-defined by ecoregion
        public static Landis.Library.Parameters.Ecoregions.AuxParm<int> AET;

        //  Minimum relative biomass for each shade class in each ecoregion
        public static Landis.Library.Parameters.Ecoregions.AuxParm<Percentage>[] MinRelativeBiomass;

        //  Maximum biomass at any site in each ecoregion
        public static Landis.Library.Parameters.Ecoregions.AuxParm<int> B_MAX;
        public static Landis.Library.Parameters.Ecoregions.AuxParm<int> ActiveSiteCount;


        //---------------------------------------------------------------------
        public static void Initialize(IInputParameters parameters)
        {
             AET = parameters.AET;
             MinRelativeBiomass = parameters.MinRelativeBiomass;

             B_MAX = new Landis.Library.Parameters.Ecoregions.AuxParm<int>(PlugIn.ModelCore.Ecoregions);
             ActiveSiteCount = new Landis.Library.Parameters.Ecoregions.AuxParm<int>(PlugIn.ModelCore.Ecoregions);

            foreach (ActiveSite site in PlugIn.ModelCore.Landscape)
            {
                IEcoregion ecoregion = PlugIn.ModelCore.Ecoregion[site];
                ActiveSiteCount[ecoregion]++;
            }
            foreach (IEcoregion ecoregion in PlugIn.ModelCore.Ecoregions)
            {
                if (EcoregionData.AET[ecoregion] <= 0.0 && ecoregion.Active)
                {
                    PlugIn.ModelCore.UI.WriteLine("   CAUTION: Ecoregion {0} has AET set to zero.", ecoregion.Name);
                }

            }
        }
        public static void UpdateB_MAX()
        {
            //AET = parameters.AET;

            //  Fill in B_MAX array
            foreach (IEcoregion ecoregion in PlugIn.ModelCore.Ecoregions)
            {
                if (!ecoregion.Active)
                    continue;

                int largest_B_MAX_Spp = 0;
                foreach (ISpecies species in PlugIn.ModelCore.Species)
                {
                    //largest_B_MAX_Spp = System.Math.Max(largest_B_MAX_Spp, SpeciesData.B_MAX_Spp[species][ecoregion]);
                    largest_B_MAX_Spp = System.Math.Max(largest_B_MAX_Spp, SpeciesData.B_MAX_Spp[species, ecoregion]);
                }
                B_MAX[ecoregion] = largest_B_MAX_Spp;
            }


        }


    }
}
