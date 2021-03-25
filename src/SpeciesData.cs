//  Authors:  Robert M. Scheller, James B. Domingo

using Landis.SpatialModeling;
using Landis.Library.BiomassCohorts;
using Landis.Core;
using System.Collections.Generic;
using Landis.Utilities;
using Landis.Library.Succession;
using Landis.Library.Biomass;

namespace Landis.Extension.Succession.Biomass
{
    public class SpeciesData
    {
        public static Dictionary<int, IDynamicInputRecord[,]> SppEcoData;

        public static Landis.Library.Parameters.Species.AuxParm<double> WoodyDebrisDecay;
        public static Landis.Library.Parameters.Species.AuxParm<double> LeafLignin;
        public static Landis.Library.Parameters.Species.AuxParm<double> LeafLongevity;
        public static Landis.Library.Parameters.Species.AuxParm<double> MortCurveShapeParm;
        public static Landis.Library.Parameters.Species.AuxParm<double> GrowthCurveShapeParm;

        //  Establishment probability for each species in each ecoregion
        public static Landis.Library.Parameters.SpeciesEcoregionAuxParm<double> EstablishProbability;
        
        //  Mortality probability for each species in each ecoregion
        public static Landis.Library.Parameters.SpeciesEcoregionAuxParm<double> MortalityProbability;

        //  Maximum ANPP for each species in each ecoregion
        public static Landis.Library.Parameters.SpeciesEcoregionAuxParm<int> ANPP_MAX_Spp;

        //  Maximum possible biomass for each species in each ecoregion
        public static Landis.Library.Parameters.SpeciesEcoregionAuxParm<int> B_MAX_Spp;

        //---------------------------------------------------------------------
        public static void Initialize(IInputParameters parameters)
        {
            LeafLignin              = parameters.LeafLignin;
            LeafLongevity           = parameters.LeafLongevity;
            MortCurveShapeParm      = parameters.MortCurveShapeParm;
            GrowthCurveShapeParm = parameters.GrowthCurveShapeParm;
            WoodyDebrisDecay = parameters.WoodyDecayRate;


        }

        public static void ChangeDynamicParameters(int year)
        {

            if(SpeciesData.SppEcoData.ContainsKey(year))
            {
                EstablishProbability = new Landis.Library.Parameters.SpeciesEcoregionAuxParm<double>(PlugIn.ModelCore.Species, PlugIn.ModelCore.Ecoregions);
                MortalityProbability = new Landis.Library.Parameters.SpeciesEcoregionAuxParm<double>(PlugIn.ModelCore.Species, PlugIn.ModelCore.Ecoregions);
                //EstablishModifier = new Landis.Library.Parameters.SpeciesEcoregionAuxParm<double>(PlugIn.ModelCore.Species, PlugIn.ModelCore.Ecoregions);
                ANPP_MAX_Spp = new Landis.Library.Parameters.SpeciesEcoregionAuxParm<int>(PlugIn.ModelCore.Species, PlugIn.ModelCore.Ecoregions);
                B_MAX_Spp = new Landis.Library.Parameters.SpeciesEcoregionAuxParm<int>(PlugIn.ModelCore.Species, PlugIn.ModelCore.Ecoregions);


                //SpeciesData.TimestepData = SpeciesData.SppEcoData[year];

                foreach(ISpecies species in PlugIn.ModelCore.Species)
                {
                    foreach(IEcoregion ecoregion in PlugIn.ModelCore.Ecoregions)
                    {
                        if (!ecoregion.Active)
                            continue;

                        if (!SpeciesData.SppEcoData.ContainsKey(year))
                            continue;

                        //if (DynamicInputs.TimestepData[species.Index, ecoregion.Index] == null)
                        //    continue;
                        
                        EstablishProbability[species,ecoregion] = SpeciesData.SppEcoData[year][species.Index, ecoregion.Index].ProbEstablish;
                        MortalityProbability[species, ecoregion] = SpeciesData.SppEcoData[year][species.Index, ecoregion.Index].ProbMortality;
                        ANPP_MAX_Spp[species,ecoregion] = SpeciesData.SppEcoData[year][species.Index, ecoregion.Index].ANPP_MAX_Spp;
                        B_MAX_Spp[species,ecoregion] = SpeciesData.SppEcoData[year][species.Index, ecoregion.Index].B_MAX_Spp;

                    }
                }

                EcoregionData.UpdateB_MAX();
            }

        }


    }
}
