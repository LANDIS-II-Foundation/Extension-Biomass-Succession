//  Copyright 2005-2010 Portland State University, University of Wisconsin
//  Authors:  Robert M. Scheller

using System;
using System.IO;
using Landis.Core;
using Landis.Library.BiomassCohorts;
using Landis.SpatialModeling;


namespace Landis.Extension.Succession.Biomass
{
    public class Outputs
    {

        //---------------------------------------------------------------------
        public static void WriteLogFile(int CurrentTime)
        {

            double[] avgLiveB       = new double[PlugIn.ModelCore.Ecoregions.Count];
            double[] avgAG_NPP      = new double[PlugIn.ModelCore.Ecoregions.Count];
            double[] avgDefoliation = new double[PlugIn.ModelCore.Ecoregions.Count];
            double[] avgLitterB     = new double[PlugIn.ModelCore.Ecoregions.Count];


            foreach (IEcoregion ecoregion in PlugIn.ModelCore.Ecoregions)
            {
                avgLiveB[ecoregion.Index] = 0.0;
                avgAG_NPP[ecoregion.Index] = 0.0;
                avgDefoliation[ecoregion.Index] = 0.0;
                avgLitterB[ecoregion.Index] = 0.0;
            }



            foreach (ActiveSite site in PlugIn.ModelCore.Landscape)
            {
                IEcoregion ecoregion = PlugIn.ModelCore.Ecoregion[site];
                int youngBiomass;  //ignored

                avgLiveB[ecoregion.Index] += Cohorts.ComputeBiomass(SiteVars.Cohorts[site], out youngBiomass);
                avgAG_NPP[ecoregion.Index]   += SiteVars.AGNPP[site];
                avgDefoliation[ecoregion.Index] += SiteVars.Defoliation[site];
                avgLitterB[ecoregion.Index] += SiteVars.Litter[site].Mass;
            }

            foreach (IEcoregion ecoregion in PlugIn.ModelCore.Ecoregions)
            {
                if(EcoregionData.ActiveSiteCount[ecoregion] > 0)
                {
                    PlugIn.summaryLog.Clear();
                    SummaryLog sl = new SummaryLog();
                    sl.Time = CurrentTime;
                    sl.EcoName = ecoregion.Name;
                    sl.ActiveCount = EcoregionData.ActiveSiteCount[ecoregion];
                    sl.AvgAG_NPP = (avgAG_NPP[ecoregion.Index] / (double)EcoregionData.ActiveSiteCount[ecoregion]);
                    sl.AvgDefoliation = (avgDefoliation[ecoregion.Index] / (double)EcoregionData.ActiveSiteCount[ecoregion]);
                    sl.AvgLitterB = (avgLitterB[ecoregion.Index] / (double)EcoregionData.ActiveSiteCount[ecoregion]);
                    sl.AvgLiveB = (avgLiveB[ecoregion.Index] / (double)EcoregionData.ActiveSiteCount[ecoregion]);

                    PlugIn.summaryLog.AddObject(sl);
                    PlugIn.summaryLog.WriteToFile();

                }
            }

            string path = MapNames.ReplaceTemplateVars("biomass-anpp-{timestep}.img", PlugIn.ModelCore.CurrentTime);
            using (IOutputRaster<IntPixel> outputRaster = PlugIn.ModelCore.CreateRaster<IntPixel>(path, PlugIn.ModelCore.Landscape.Dimensions))
            {
                IntPixel pixel = outputRaster.BufferPixel;
                foreach (Site site in PlugIn.ModelCore.Landscape.AllSites)
                {
                    if (site.IsActive)
                    {
                        pixel.MapCode.Value = (int)SiteVars.AGNPP[site];
                    }
                    else
                    {
                        //  Inactive site
                        pixel.MapCode.Value = 0;
                    }
                    outputRaster.WriteBufferPixel();
                }
            }
        }

    }
}
