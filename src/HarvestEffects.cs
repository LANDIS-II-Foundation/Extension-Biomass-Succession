//  Author: Robert Scheller, Melissa Lucash

using Landis.SpatialModeling;
using Landis.Utilities;


namespace Landis.Extension.Succession.Biomass
{
    /// <summary>
    /// A helper class.
    /// </summary>
    public class HarvestReductions
    {
        private string prescription;
        private double coarseLitterReduction;
        private double fineLitterReduction;
        //private double somReduction;
        private double cohortWoodReduction;
        private double cohortLeafReduction;

        public string Name
        {
            get
            {
                return prescription;
            }
            set
            {
                if (value != null)
                    prescription = value;
            }
        }
        public double CoarseLitterReduction
        {
            get
            {
                return coarseLitterReduction;
            }
            set
            {
                if (value < 0.0 || value > 1.0)
                    throw new InputValueException(value.ToString(), "Coarse litter reduction due to fire must be between 0 and 1.0");
                coarseLitterReduction = value;
            }

        }
        public double FineLitterReduction
        {
            get
            {
                return fineLitterReduction;
            }
            set
            {
                if (value < 0.0 || value > 1.0)
                    throw new InputValueException(value.ToString(), "Fine litter reduction due to fire must be between 0 and 1.0");
                fineLitterReduction = value;
            }

        }
        public double CohortWoodReduction
        {
            get
            {
                return cohortWoodReduction;
            }
            set
            {
                if (value < 0.0 || value > 1.0)
                    throw new InputValueException(value.ToString(), "Cohort wood reduction due to fire must be between 0 and 1.0");
                cohortWoodReduction = value;
            }

        }
        public double CohortLeafReduction
        {
            get
            {
                return cohortLeafReduction;
            }
            set
            {
                if (value < 0.0 || value > 1.0)
                    throw new InputValueException(value.ToString(), "Cohort wood reduction due to fire must be between 0 and 1.0");
                cohortLeafReduction = value;
            }

        }
        //public double SOMReduction
        //{
        //    get
        //    {
        //        return somReduction;
        //    }
        //    set
        //    {
        //        if (value < 0.0 || value > 1.0)
        //            throw new InputValueException(value.ToString(), "Soil Organic Matter (SOM) reduction due to fire must be between 0 and 1.0");
        //        somReduction = value;
        //    }

        //}

        //---------------------------------------------------------------------
        public HarvestReductions()
        {
            this.prescription = "";
            this.CoarseLitterReduction = 0.0;
            this.FineLitterReduction = 0.0;
            this.CohortLeafReduction = 0.0;
            this.CohortWoodReduction = 0.0;
            //this.SOMReduction = 0.0;
        }
    }

    public class HarvestEffects
    {

        public static double GetCohortWoodRemovalFraction(ActiveSite site)
        {

            double woodRemoval = 1.0;  // Default is 100% removal
            if (SiteVars.HarvestPrescriptionName == null)
                return woodRemoval;

            foreach (HarvestReductions prescription in PlugIn.Parameters.HarvestReductionsTable)
            {
                if (ComparePrescriptionNames(prescription.Name, site))
                {
                    woodRemoval = prescription.CohortWoodReduction;
                }
            }

            return woodRemoval;

        }

        public static double GetCohortLeafRemovalFraction(ActiveSite site)
        {
            double leafRemoval = 0.0;  // Default is 0% removal
            if (SiteVars.HarvestPrescriptionName == null)
                return leafRemoval;

            foreach (HarvestReductions prescription in PlugIn.Parameters.HarvestReductionsTable)
            {
                if (ComparePrescriptionNames(prescription.Name, site))
                {
                    leafRemoval = prescription.CohortLeafReduction;
                }
            }

            return leafRemoval;

        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Computes fire effects on litter, coarse woody debris, duff layer.
        /// </summary>
        public static void ReduceLayers(Site site)
        {
            //PlugIn.ModelCore.UI.WriteLine("   Calculating harvest induced layer reductions...");

            double litterLossMultiplier = 0.0;
            double woodLossMultiplier = 0.0;
            //double som_Multiplier = 0.0;

            string harvestPrescriptionName = SiteVars.HarvestPrescriptionName[site];

            bool found = false;
            foreach (HarvestReductions prescriptionTableEntry in PlugIn.Parameters.HarvestReductionsTable)
            {
                if (SiteVars.HarvestPrescriptionName != null && ComparePrescriptionNames(prescriptionTableEntry.Name, site))
                    //prescriptionName.Trim() == prescriptionTableEntry.Name.Trim())
                {
                    litterLossMultiplier = prescriptionTableEntry.FineLitterReduction;
                    woodLossMultiplier = prescriptionTableEntry.CoarseLitterReduction;
                    //som_Multiplier = prescription.SOMReduction;

                    found = true;
                }
            }
            if (!found)
            {
                PlugIn.ModelCore.UI.WriteLine("   WARNING: Prescription {0} not found in the Biomass Succession Harvest Effects Table", harvestPrescriptionName);
                return;
            }
            //PlugIn.ModelCore.UI.WriteLine("   LitterLoss={0:0.00}, woodLoss={1:0.00}, SOM_loss={2:0.00}, SITE={3}", litterLossMultiplier, woodLossMultiplier, som_Multiplier, site);

            // litter first
            SiteVars.Litter[site].ReduceMass(litterLossMultiplier);

            // Surface dead wood

            SiteVars.WoodyDebris[site].ReduceMass(woodLossMultiplier);


        }

        public static bool ComparePrescriptionNames(string prescriptionTableName, Site site)
        {
            string harvestPrescriptionName = SiteVars.HarvestPrescriptionName[site].Trim();
            string tablePrescriptionName = prescriptionTableName.Trim();

            if (harvestPrescriptionName == tablePrescriptionName)
                return true;
            else if (tablePrescriptionName.EndsWith("*"))
            {
                if (harvestPrescriptionName.Contains(tablePrescriptionName.TrimEnd('*')))
                    return true;
                else
                    return false;
            }
            else
                return false;
        }


    }
}
