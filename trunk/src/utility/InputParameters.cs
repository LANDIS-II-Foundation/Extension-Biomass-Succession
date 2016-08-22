//  Copyright 2005-2010 Portland State University, University of Wisconsin
//  Authors:  Robert M. Scheller, James B. Domingo

using System.Collections.Generic;
using System.Diagnostics;
using Edu.Wisc.Forest.Flel.Util;
using Landis.Core;
using Landis.Library.Succession;
using Landis.Library.Parameters;

namespace Landis.Extension.Succession.Biomass
{

    /// <summary>
    /// The parameters for biomass succession.
    /// </summary>
    public class InputParameters
        : IInputParameters 
    {
        private int timestep;
        private SeedingAlgorithms seedAlg;
        private bool calibrateMode;
        private double spinupMortalityFraction;

        private List<ISufficientLight> sufficientLight;

        private Landis.Library.Parameters.Species.AuxParm<double> leafLongevity;
        private Landis.Library.Parameters.Species.AuxParm<double> woodyDecayRate;
        private Landis.Library.Parameters.Species.AuxParm<double> mortCurveShapeParm;
        private Landis.Library.Parameters.Species.AuxParm<double> growthCurveShapeParm;
        private Landis.Library.Parameters.Species.AuxParm<double> leafLignin;
        private Landis.Library.Parameters.Ecoregions.AuxParm<int> aet;
        private Landis.Library.Parameters.Ecoregions.AuxParm<Percentage>[] minRelativeBiomass;

        private string dynamicInputFile;
        private string ageOnlyDisturbanceParms;
        private string initCommunities;
        private string communitiesMap;

        //---------------------------------------------------------------------
        /// <summary>
        /// Timestep (years)
        /// </summary>

        public int Timestep
        {
            get {
                return timestep;
            }
            set {
                if (value < 0)
                    throw new InputValueException(value.ToString(), "Timestep must be > or = 0");
                timestep = value;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Seeding algorithm
        /// </summary>
        public SeedingAlgorithms SeedAlgorithm
        {
            get {
                return seedAlg;
            }
            set {
                seedAlg = value;
            }
        }
        //---------------------------------------------------------------------

        public Landis.Library.Parameters.Ecoregions.AuxParm<Percentage>[] MinRelativeBiomass
        {
            get
            {
                return minRelativeBiomass;
            }
        }
        //---------------------------------------------------------------------

        /// <summary>
        /// Path to the file with the initial communities' definitions.
        /// </summary>
        public string InitialCommunities
        {
            get
            {
                return initCommunities;
            }

            set
            {
                if (value != null)
                {
                    ValidatePath(value);
                }
                initCommunities = value;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Path to the raster file showing where the initial communities are.
        /// </summary>
        public string InitialCommunitiesMap
        {
            get
            {
                return communitiesMap;
            }

            set
            {
                if (value != null)
                {
                    ValidatePath(value);
                }
                communitiesMap = value;
            }
        }
        //---------------------------------------------------------------------
        public bool CalibrateMode
        {
            get {
                return calibrateMode;
            }
            set {
                calibrateMode = value;
            }
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Background mortality rates applied during the biomass spin-up phase.
        /// This represents background disturbance before year 1.
        /// </summary>

        public double SpinupMortalityFraction
        {
            get {
                return spinupMortalityFraction;
            }
            set {
                if (value < 0.0 || value > 0.5)
                        throw new InputValueException(value.ToString(), "SpinupMortalityFraction must be > 0.0 and < 0.5");
                spinupMortalityFraction = value;
            }
        }
        //---------------------------------------------------------------------
        public void SetMinRelativeBiomass(byte shadeClass,
                                          IEcoregion ecoregion,
                                          InputValue<Percentage> newValue)
        {
            Debug.Assert(1 <= shadeClass && shadeClass <= 5);
            Debug.Assert(ecoregion != null);
            if (newValue != null)
            {
                if (newValue.Actual < 0.0 || newValue.Actual > 1.0)
                    throw new InputValueException(newValue.String,
                                                  "{0} is not between 0% and 100%", newValue.String);
            }
            
            minRelativeBiomass[shadeClass][ecoregion] = newValue;
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Definitions of sufficient light probabilities.
        /// </summary>
        public List<ISufficientLight> LightClassProbabilities
        {
            get {
                return sufficientLight;
            }
            set
            {
                Debug.Assert(sufficientLight.Count != 0);
                sufficientLight = value;
            }
        }
        //---------------------------------------------------------------------

        public Landis.Library.Parameters.Species.AuxParm<double> LeafLongevity
        {
            get {
                return leafLongevity;
            }
        }

        //---------------------------------------------------------------------

        public Landis.Library.Parameters.Species.AuxParm<double> WoodyDecayRate
        {
            get {
                return woodyDecayRate;
            }
        }

        //---------------------------------------------------------------------

        public Landis.Library.Parameters.Species.AuxParm<double> MortCurveShapeParm
        {
            get {
                return mortCurveShapeParm;
            }
        }

        //---------------------------------------------------------------------

        public Landis.Library.Parameters.Species.AuxParm<double> GrowthCurveShapeParm
        {
            get
            {
                return growthCurveShapeParm;
            }
        }
        //---------------------------------------------------------------------

        public Landis.Library.Parameters.Species.AuxParm<double> LeafLignin
        {
            get {
                return leafLignin;
            }
        }
        
        //---------------------------------------------------------------------

        public Landis.Library.Parameters.Ecoregions.AuxParm<int> AET
        {
            get {
                return aet;
            }

        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Input file for the dynamic inputs
        /// </summary>
        public string DynamicInputFile
        {
            get
            {
                return dynamicInputFile;
            }
            set
            {
                dynamicInputFile = value;
            }
        }
        //---------------------------------------------------------------------
        public void SetLeafLongevity(ISpecies           species,
                                     InputValue<double> newValue)
        {
            Debug.Assert(species != null);
            leafLongevity[species] = newValue.CheckInRange(1.0, 10.0, "leafLongevity");
        }

        //---------------------------------------------------------------------

        public void SetWoodyDecayRate(ISpecies           species,
                                     InputValue<double> newValue)
        {
            Debug.Assert(species != null);
            woodyDecayRate[species] = newValue.CheckInRange(0.0, 1.0, "woodyDecayRate");
        }

        //---------------------------------------------------------------------

        public void SetMortCurveShapeParm(ISpecies           species,
                                          InputValue<double> newValue)
        {
            Debug.Assert(species != null);
            mortCurveShapeParm[species] = newValue.CheckInRange(5.0, 25.0, "mortCurveShapeParm"); 
        }

        //---------------------------------------------------------------------

        public void SetGrowthCurveShapeParm(ISpecies species,
                                          InputValue<double> newValue)
        {
            Debug.Assert(species != null);
            growthCurveShapeParm[species] = newValue.CheckInRange(0, 1.0, "growthCurveShapeParm"); 
        }
        //---------------------------------------------------------------------

        public void SetLeafLignin(ISpecies           species,
                                          InputValue<double> newValue)
        {
            Debug.Assert(species != null);
            leafLignin[species] = newValue.CheckInRange(0, 0.4, "leafLignin"); 
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Path to the optional file with the biomass parameters for age-only
        /// disturbances.
        /// </summary>
        public string AgeOnlyDisturbanceParms
        {
            get {
                return ageOnlyDisturbanceParms;
            }
            set {
                string path = value;
                if (path.Trim(null).Length == 0)
                    throw new InputValueException(path, "\"{0}\" is not a valid path.", path);
                ageOnlyDisturbanceParms = value;
            }
        }

        //---------------------------------------------------------------------

        public void SetAET(IEcoregion           ecoregion,
                                          InputValue<int> newValue)
        {
            Debug.Assert(ecoregion != null);
            aet[ecoregion] = newValue.CheckInRange(0, 10000, "aet");  //FIXME:  FIND GOOD MAXIMUM
        }
        //---------------------------------------------------------------------

        public InputParameters()
        {
            sufficientLight = new List<ISufficientLight>();
            leafLongevity = new Landis.Library.Parameters.Species.AuxParm<double>(PlugIn.ModelCore.Species);
            woodyDecayRate = new Landis.Library.Parameters.Species.AuxParm<double>(PlugIn.ModelCore.Species);
            mortCurveShapeParm = new Landis.Library.Parameters.Species.AuxParm<double>(PlugIn.ModelCore.Species);
            growthCurveShapeParm = new Landis.Library.Parameters.Species.AuxParm<double>(PlugIn.ModelCore.Species);
            leafLignin = new Landis.Library.Parameters.Species.AuxParm<double>(PlugIn.ModelCore.Species);
            //maxLAI = new Species.AuxParm<double>(PlugIn.ModelCore.Species);
            //lightExtinctionCoeff = new Species.AuxParm<double>(PlugIn.ModelCore.Species);
            //pctBioMaxLAI = new Species.AuxParm<double>(PlugIn.ModelCore.Species);
            aet = new Landis.Library.Parameters.Ecoregions.AuxParm<int>(PlugIn.ModelCore.Ecoregions);

            minRelativeBiomass = new Landis.Library.Parameters.Ecoregions.AuxParm<Percentage>[6];
            for (byte shadeClass = 1; shadeClass <= 5; shadeClass++)
            {
                minRelativeBiomass[shadeClass] = new Landis.Library.Parameters.Ecoregions.AuxParm<Percentage>(PlugIn.ModelCore.Ecoregions);
            }
        }
        //---------------------------------------------------------------------

        private void ValidatePath(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new InputValueException();
            if (path.Trim(null).Length == 0)
                throw new InputValueException(path,
                                              "\"{0}\" is not a valid path.",
                                              path);
        }

    }
}
