//  Author: Robert Scheller, Melissa Lucash

using Landis.Core;
using Landis.SpatialModeling;
using Landis.Utilities;
using Landis.Library.Succession;
using Landis.Library.Climate;
using System.Collections.Generic;
using System.Linq;
using System;


namespace Landis.Extension.Succession.Biomass
{
    public class ClimateRegionData
    {

        public static Landis.Library.Parameters.Ecoregions.AuxParm<int> ActiveSiteCount;
        public static Landis.Library.Parameters.Ecoregions.AuxParm<double> AnnualNDeposition;    
        public static Landis.Library.Parameters.Ecoregions.AuxParm<double[]> MonthlyNDeposition; 
        public static Landis.Library.Parameters.Ecoregions.AuxParm<AnnualClimate_Monthly> AnnualWeather;

        //---------------------------------------------------------------------
        public static void Initialize(IInputParameters parameters)
        {
            ActiveSiteCount = new Landis.Library.Parameters.Ecoregions.AuxParm<int>(PlugIn.ModelCore.Ecoregions);
            AnnualWeather = new Landis.Library.Parameters.Ecoregions.AuxParm<AnnualClimate_Monthly>(PlugIn.ModelCore.Ecoregions);
            MonthlyNDeposition = new Landis.Library.Parameters.Ecoregions.AuxParm<double[]>(PlugIn.ModelCore.Ecoregions);

            AnnualNDeposition = new Landis.Library.Parameters.Ecoregions.AuxParm<double>(PlugIn.ModelCore.Ecoregions);
            
            foreach (ActiveSite site in PlugIn.ModelCore.Landscape)
            {
                IEcoregion ecoregion = PlugIn.ModelCore.Ecoregion[site];
                ActiveSiteCount[ecoregion]++;
            }

            foreach (IEcoregion ecoregion in PlugIn.ModelCore.Ecoregions)
            {
                MonthlyNDeposition[ecoregion] = new double[12];

                if (ecoregion.Active)
                {
                    Climate.GenerateEcoregionClimateData(ecoregion, 0, 45); // PlugIn.Parameters.Latitude); RMS TESTING
                    SetSingleAnnualClimate(ecoregion, 0, Climate.Phase.SpinUp_Climate);  // Some placeholder data to get things started.
                }
            }

            
        }
        ////---------------------------------------------------------------------
        //public static void ChangeParameters(Dynamic.IParameters parameters)
        //{
        //    B_MAX               = new Ecoregions.AuxParm<int>(PlugIn.ModelCore.Ecoregions);
            
        //    //  Fill in B_MAX array
        //    foreach (IEcoregion ecoregion in PlugIn.ModelCore.Ecoregions) 
        //    {
        //        if(ecoregion.Active)
        //        {
        //            int largest_B_MAX_Spp = 0;
        //            foreach (ISpecies species in PlugIn.ModelCore.Species) 
        //            {
        //                largest_B_MAX_Spp = Math.Max(largest_B_MAX_Spp, SpeciesData.B_MAX_Spp[species][ecoregion]);
        //                //PlugIn.ModelCore.UI.WriteLine("B_MAX={0}. species={1}, ecoregion={2}", largest_B_MAX_Spp, species.Name, ecoregion.Name);
        //            }
        //            B_MAX[ecoregion] = largest_B_MAX_Spp;
        //        }
        //    }
         
        //}

        //---------------------------------------------------------------------
        // Generates new climate parameters for a SINGLE ECOREGION at an annual time step.
        public static void SetSingleAnnualClimate(IEcoregion ecoregion, int year, Climate.Phase spinupOrfuture)
        {
            int actualYear = Climate.Future_MonthlyData.Keys.Min() + year;

            if (spinupOrfuture == Climate.Phase.Future_Climate)
            {
                //PlugIn.ModelCore.UI.WriteLine("Retrieving {0} for year {1}.", spinupOrfuture.ToString(), actualYear);
                if (Climate.Future_MonthlyData.ContainsKey(actualYear))
                {
                    AnnualWeather[ecoregion] = Climate.Future_MonthlyData[actualYear][ecoregion.Index];
                }
                //else
                //    PlugIn.ModelCore.UI.WriteLine("Key is missing: Retrieving {0} for year {1}.", spinupOrfuture.ToString(), actualYear);
            }
            else
            {
                //PlugIn.ModelCore.UI.WriteLine("Retrieving {0} for year {1}.", spinupOrfuture.ToString(), actualYear);
                if (Climate.Spinup_MonthlyData.ContainsKey(actualYear))
                {
                    AnnualWeather[ecoregion] = Climate.Spinup_MonthlyData[actualYear][ecoregion.Index];
                }
            }
           
        }

        //---------------------------------------------------------------------
        // Generates new climate parameters for all ecoregions at an annual time step.
        public static void SetAllEcoregions_FutureAnnualClimate(int year)
        {
            int actualYear = Climate.Future_MonthlyData.Keys.Min() + year - 1;
            foreach (IEcoregion ecoregion in PlugIn.ModelCore.Ecoregions)
            {
                if (ecoregion.Active)
                {
                    //PlugIn.ModelCore.UI.WriteLine("Retrieving {0} for year {1}.", spinupOrfuture.ToString(), actualYear);
                    if (Climate.Future_MonthlyData.ContainsKey(actualYear))
                    {
                        AnnualWeather[ecoregion] = Climate.Future_MonthlyData[actualYear][ecoregion.Index];
                    }

                    //PlugIn.ModelCore.UI.WriteLine("Utilizing Climate Data: Simulated Year = {0}, actualClimateYearUsed = {1}.", actualYear, AnnualWeather[ecoregion].Year);
                }

            }
        }
        

        
    }
}
