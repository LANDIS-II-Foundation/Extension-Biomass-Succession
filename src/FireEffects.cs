//  Author: Robert Scheller, Melissa Lucash

using Landis.Core;
using Landis.SpatialModeling;
using Landis.Utilities;
using Landis.Library.BiomassCohorts;  

using System;
using System.Collections.Generic;


namespace Landis.Extension.Succession.Biomass
{
    /// <summary>
    /// A helper class.
    /// </summary>
    public class FireReductions
    {
        private double coarseLitterReduction;
        private double fineLitterReduction;
        
        public double CoarseLitterReduction
        {
            get {
                return coarseLitterReduction; 
            }
            set {
                if (value < 0.0 || value > 1.0)
                    throw new InputValueException(value.ToString(), "Coarse litter reduction due to fire must be between 0 and 1.0");
                coarseLitterReduction = value;
            }
               
        }
        public double FineLitterReduction
        {
            get {
                return fineLitterReduction; 
            }
            set {
                if (value < 0.0 || value > 1.0)
                    throw new InputValueException(value.ToString(), "Fine litter reduction due to fire must be between 0 and 1.0");
                fineLitterReduction = value;
            }
               
        }

        //---------------------------------------------------------------------
        public FireReductions()
        {
            this.CoarseLitterReduction = 0.0; 
            this.FineLitterReduction = 0.0;
        }
    }
    
    public class FireEffects
    {
        public static FireReductions[] ReductionsTable; 
        
        public FireEffects(int numberOfSeverities)
        {
            ReductionsTable = new FireReductions[numberOfSeverities+1];  //will ignore zero
            
            for(int i=0; i <= numberOfSeverities; i++)
            {
                ReductionsTable[i] = new FireReductions();
            }
        }
       

        //---------------------------------------------------------------------

        public static void Initialize(IInputParameters parameters)
        {
            ReductionsTable = parameters.FireReductionsTable; 
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Computes fire effects on litter, coarse woody debris, mineral soil, and charcoal.
        ///   No effects on soil organic matter (negligible according to Johnson et al. 2001).
        /// </summary>
        public static void ReduceLayers(byte severity, Site site)
        {
            //PlugIn.ModelCore.UI.WriteLine("   Calculating fire induced layer reductions. Severity={0}.", severity);
            //PlugIn.ModelCore.UI.WriteLine("   Calculating fire induced layer reductions. FineLitter={0}, Wood={1}.", ReductionsTable[severity].FineLitterReduction, ReductionsTable[severity].CoarseLitterReduction);

            // litter first
            double fineLitterReduction = ReductionsTable[severity].FineLitterReduction;
            SiteVars.Litter[site].ReduceMass(fineLitterReduction);

            // Surface dead wood

            double woodLossMultiplier = ReductionsTable[severity].CoarseLitterReduction;
            SiteVars.WoodyDebris[site].ReduceMass(woodLossMultiplier);
            

        }
        //---------------------------------------------------------------------

        //// Crown scorching is when a cohort loses its foliage but is not killed.
        //public static double CrownScorching(ICohort cohort, byte siteSeverity)
        //{
        
        //    int difference = (int) siteSeverity - cohort.Species.FireTolerance;
        //    double ageFraction = 1.0 - ((double) cohort.Age / (double) cohort.Species.Longevity);
            
        //    if(SpeciesData.Epicormic[cohort.Species])
        //    {
        //        if(difference < 0)
        //            return 0.5 * ageFraction;
        //        if(difference == 0)
        //            return 0.75 * ageFraction;
        //        if(difference > 0)
        //            return 1.0 * ageFraction;
        //    }
            
        //    return 0.0;
        //}

    }
}
