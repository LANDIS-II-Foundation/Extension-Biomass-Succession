//  Copyright 2005-2010 Portland State University, University of Wisconsin
//  Authors:  Robert M. Scheller, James B. Domingo

using Landis.Library.Succession;
using Landis.Core;
using System.Collections.Generic;
using Landis.Utilities;

namespace Landis.Extension.Succession.Biomass
{
    /// <summary>
    /// A parser that reads biomass succession parameters from text input.
    /// </summary>
    public class InputParametersParser
        : TextParser<IInputParameters>
    {
        public static class Names
        {
            public const string Timestep = "Timestep";
            public const string SeedingAlgorithm = "SeedingAlgorithm";
            public const string ClimateConfigFile = "ClimateConfigFile";
            public const string AgeOnlyDisturbanceParms = "AgeOnlyDisturbances:BiomassParameters";
            public const string DynamicInputFile = "DynamicInputFile";
            public const string CalibrateMode = "CalibrateMode";
        }
        //---------------------------------------------------------------------
        public override string LandisDataValue
        {
            get
            {
                return PlugIn.ExtensionName;
            }
        }
        //---------------------------------------------------------------------

        private Dictionary<string, int> speciesLineNums;
        private InputVar<string> speciesName;


        //---------------------------------------------------------------------

        static InputParametersParser()
        {
            Percentage dummy = new Percentage();
            SeedingAlgorithmsUtil.RegisterForInputValues();
        }

        //---------------------------------------------------------------------

        public InputParametersParser()
        {
            this.speciesLineNums = new Dictionary<string, int>();
            this.speciesName = new InputVar<string>("Species");

        }

        //---------------------------------------------------------------------

        protected override IInputParameters Parse()
        {
            //InputVar<string> landisData = new InputVar<string>("LandisData");
            //ReadVar(landisData);
            //if (landisData.Value.Actual != PlugIn.ExtensionName)
            //    throw new InputValueException(landisData.Value.String, "The value is not \"{0}\"", PlugIn.ExtensionName);
            
            ReadLandisDataVar();
            InputParameters parameters = new InputParameters();

            InputVar<int> timestep = new InputVar<int>(Names.Timestep);
            ReadVar(timestep);
            parameters.Timestep = timestep.Value;

            InputVar<SeedingAlgorithms> seedAlg = new InputVar<SeedingAlgorithms>(Names.SeedingAlgorithm);
            ReadVar(seedAlg);
            parameters.SeedAlgorithm = seedAlg.Value;

            //---------------------------------------------------------------------------------

            InputVar<string> initCommunities = new InputVar<string>("InitialCommunities");
            ReadVar(initCommunities);
            parameters.InitialCommunities = initCommunities.Value;

            InputVar<string> communitiesMap = new InputVar<string>("InitialCommunitiesMap");
            ReadVar(communitiesMap);
            parameters.InitialCommunitiesMap = communitiesMap.Value;

            InputVar<string> climateConfigFile = new InputVar<string>(Names.ClimateConfigFile);
            ReadVar(climateConfigFile);
            parameters.ClimateConfigFile = climateConfigFile.Value;

            //---------------------------------------------------------------------------------
            InputVar<bool> calimode = new InputVar<bool>(Names.CalibrateMode);
            if(ReadOptionalVar(calimode))
                parameters.CalibrateMode = calimode.Value;
            else
                parameters.CalibrateMode = false;

            InputVar<double> spinMort = new InputVar<double>("SpinupMortalityFraction");
            if(ReadOptionalVar(spinMort))
                parameters.SpinupMortalityFraction = spinMort.Value;
            else
                parameters.SpinupMortalityFraction = 0.0;

            //--------------------------
            //  MinRelativeBiomass table

            ReadName("MinRelativeBiomass");

            const string SufficientLight = "SufficientLight";

            List<IEcoregion> ecoregions = ReadEcoregions();
            string lastEcoregion = ecoregions[ecoregions.Count - 1].Name;

            InputVar<byte> shadeClassVar = new InputVar<byte>("Shade Class");
            for (byte shadeClass = 1; shadeClass <= 5; shadeClass++)
            {
                if (AtEndOfInput)
                    throw NewParseException("Expected a line with shade class {0}", shadeClass);

                StringReader currentLine = new StringReader(CurrentLine);
                ReadValue(shadeClassVar, currentLine);
                if (shadeClassVar.Value.Actual != shadeClass)
                    throw new InputValueException(shadeClassVar.Value.String,
                                                  "Expected the shade class {0}", shadeClass);

                foreach (IEcoregion ecoregion in ecoregions)
                {
                    InputVar<Percentage> MinRelativeBiomass = new InputVar<Percentage>("Ecoregion " + ecoregion.Name);
                    ReadValue(MinRelativeBiomass, currentLine);
                    parameters.SetMinRelativeBiomass(shadeClass, ecoregion, MinRelativeBiomass.Value);
                }

                CheckNoDataAfter("the Ecoregion " + lastEcoregion + " column",
                                 currentLine);
                GetNextLine();
            }
            //----------------------------------------------------------
            //  Read table of sufficient light probabilities.
            //  Shade classes are in increasing order.

            ReadName(SufficientLight);
            const string SpeciesParameters = "SpeciesParameters";


            InputVar<byte> sc = new InputVar<byte>("Shade Class");
            InputVar<double> pl0 = new InputVar<double>("Probability of Germination - Light Level 0");
            InputVar<double> pl1 = new InputVar<double>("Probability of Germination - Light Level 1");
            InputVar<double> pl2 = new InputVar<double>("Probability of Germination - Light Level 2");
            InputVar<double> pl3 = new InputVar<double>("Probability of Germination - Light Level 3");
            InputVar<double> pl4 = new InputVar<double>("Probability of Germination - Light Level 4");
            InputVar<double> pl5 = new InputVar<double>("Probability of Germination - Light Level 5");

            int previousNumber = 0;


            while (! AtEndOfInput && CurrentName != SpeciesParameters
                                  && previousNumber != 6) {
                StringReader currentLine = new StringReader(CurrentLine);

                ISufficientLight suffLight = new SufficientLight();

                parameters.LightClassProbabilities.Add(suffLight);

                ReadValue(sc, currentLine);
                suffLight.ShadeClass = sc.Value;

                //  Check that the current shade class is 1 more than
                //  the previous number (numbers are must be in increasing order).
                if (sc.Value.Actual != (byte) previousNumber + 1)
                    throw new InputValueException(sc.Value.String,
                                                  "Expected the severity number {0}",
                                                  previousNumber + 1);
                previousNumber = (int) sc.Value.Actual;

                ReadValue(pl0, currentLine);
                suffLight.ProbabilityLight0 = pl0.Value;

                ReadValue(pl1, currentLine);
                suffLight.ProbabilityLight1 = pl1.Value;

                ReadValue(pl2, currentLine);
                suffLight.ProbabilityLight2 = pl2.Value;

                ReadValue(pl3, currentLine);
                suffLight.ProbabilityLight3 = pl3.Value;

                ReadValue(pl4, currentLine);
                suffLight.ProbabilityLight4 = pl4.Value;

                ReadValue(pl5, currentLine);
                suffLight.ProbabilityLight5 = pl5.Value;


                CheckNoDataAfter("the " + pl5.Name + " column",
                                 currentLine);
                GetNextLine();
            }
            if (parameters.LightClassProbabilities.Count == 0)
                throw NewParseException("No sufficient light probabilities defined.");
            if (previousNumber != 5)
                throw NewParseException("Expected shade class {0}", previousNumber + 1);

            //-------------------------
            //  SpeciesParameters table

            ReadName("SpeciesParameters");
            const string EcoregionParameters = "EcoregionParameters";

            speciesLineNums.Clear();  //  If parser re-used (i.e., for testing purposes)

            InputVar<double> leafLongevity = new InputVar<double>("Leaf Longevity");
            InputVar<double> woodyDecayRate = new InputVar<double>("Woody Decay Rate");
            InputVar<double> mortCurveShapeParm = new InputVar<double>("Mortality Curve Shape Parameter");
            InputVar<double> growthCurveShapeParm = new InputVar<double>("Mortality Curve Shape Parameter");
            InputVar<double> leafLignin = new InputVar<double>("Leaf Percent Lignin");
            InputVar<double> maxlai = new InputVar<double>("Maximum LAI");
            InputVar<double> lec = new InputVar<double>("Light extinction coefficient");
            InputVar<double> pctBio = new InputVar<double>("Pct Biomass Max LAI");
            //string lastColumn = "the " + mortCurveShapeParm.Name + " column";

            while (! AtEndOfInput && CurrentName != EcoregionParameters) {
                StringReader currentLine = new StringReader(CurrentLine);
                ISpecies species = ReadSpecies(currentLine);

                ReadValue(leafLongevity, currentLine);
                parameters.SetLeafLongevity(species, leafLongevity.Value);

                ReadValue(woodyDecayRate, currentLine);
                parameters.SetWoodyDecayRate(species, woodyDecayRate.Value);

                ReadValue(mortCurveShapeParm, currentLine);
                parameters.SetMortCurveShapeParm(species, mortCurveShapeParm.Value);

                ReadValue(growthCurveShapeParm, currentLine);
                parameters.SetGrowthCurveShapeParm(species, growthCurveShapeParm.Value);

                ReadValue(leafLignin, currentLine);
                parameters.SetLeafLignin(species, leafLignin.Value);
                
                CheckNoDataAfter(leafLignin.Name, currentLine);
                GetNextLine();
            }

            ReadName("EcoregionParameters");

            InputVar<string> ecoregionName = new InputVar<string>("Ecoregion Name");
            InputVar<int> aet = new InputVar<int>("Actual Evapotranspiration");
            Dictionary <string, int> lineNumbers = new Dictionary<string, int>();

            string lastColumn = "the " + aet.Name + " column";

            while (! AtEndOfInput && CurrentName != Names.DynamicInputFile) {
                StringReader currentLine = new StringReader(CurrentLine);

                ReadValue(ecoregionName, currentLine);

                IEcoregion ecoregion = GetEcoregion(ecoregionName.Value,
                                                    lineNumbers);

                ReadValue(aet, currentLine);
                parameters.SetAET(ecoregion, aet.Value);

                CheckNoDataAfter(lastColumn, currentLine);
                GetNextLine();
            }


            InputVar<string> dynInputFile = new InputVar<string>(Names.DynamicInputFile);
            ReadVar(dynInputFile);
            parameters.DynamicInputFile = dynInputFile.Value;


            string lastParameter = null;
            if (! AtEndOfInput && CurrentName == Names.AgeOnlyDisturbanceParms) {
                InputVar<string> ageOnlyDisturbanceParms = new InputVar<string>(Names.AgeOnlyDisturbanceParms);
                ReadVar(ageOnlyDisturbanceParms);
                parameters.AgeOnlyDisturbanceParms = ageOnlyDisturbanceParms.Value;

                lastParameter = "the " + Names.AgeOnlyDisturbanceParms + " parameter";
            }

            if (lastParameter != null)
                CheckNoDataAfter(lastParameter);

            return parameters;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Reads a species name from the current line, and verifies the name.
        /// </summary>
        private ISpecies ReadSpecies(StringReader currentLine)
        {
            ReadValue(speciesName, currentLine);
            ISpecies species = PlugIn.ModelCore.Species[speciesName.Value.Actual];
            if (species == null)
                throw new InputValueException(speciesName.Value.String,
                                              "{0} is not a species name.",
                                              speciesName.Value.String);
            int lineNumber;
            if (speciesLineNums.TryGetValue(species.Name, out lineNumber))
                throw new InputValueException(speciesName.Value.String,
                                              "The species {0} was previously used on line {1}",
                                              speciesName.Value.String, lineNumber);
            else
                speciesLineNums[species.Name] = LineNumber;
            return species;
        }
        //---------------------------------------------------------------------

        private IEcoregion GetEcoregion(InputValue<string>      ecoregionName,
                                        Dictionary<string, int> lineNumbers)
        {
            IEcoregion ecoregion = PlugIn.ModelCore.Ecoregions[ecoregionName.Actual];
            if (ecoregion == null)
                throw new InputValueException(ecoregionName.String,
                                              "{0} is not an ecoregion name.",
                                              ecoregionName.String);
            int lineNumber;
            if (lineNumbers.TryGetValue(ecoregion.Name, out lineNumber))
                throw new InputValueException(ecoregionName.String,
                                              "The ecoregion {0} was previously used on line {1}",
                                              ecoregionName.String, lineNumber);
            else
                lineNumbers[ecoregion.Name] = LineNumber;

            return ecoregion;
        }
        //---------------------------------------------------------------------

        /// <summary>
        /// Reads ecoregion names as column headings
        /// </summary>
        private List<IEcoregion> ReadEcoregions()
        {
            if (AtEndOfInput)
                throw NewParseException("Expected a line with the names of 1 or more active ecoregions.");

            InputVar<string> ecoregionName = new InputVar<string>("Ecoregion");
            List<IEcoregion> ecoregions = new List<IEcoregion>();
            StringReader currentLine = new StringReader(CurrentLine);
            TextReader.SkipWhitespace(currentLine);
            while (currentLine.Peek() != -1)
            {
                ReadValue(ecoregionName, currentLine);
                IEcoregion ecoregion = PlugIn.ModelCore.Ecoregions[ecoregionName.Value.Actual];
                if (ecoregion == null)
                    throw new InputValueException(ecoregionName.Value.String,
                                                  "{0} is not an ecoregion name.",
                                                  ecoregionName.Value.String);
                if (!ecoregion.Active)
                    throw new InputValueException(ecoregionName.Value.String,
                                                  "{0} is not an active ecoregion",
                                                  ecoregionName.Value.String);
                if (ecoregions.Contains(ecoregion))
                    throw new InputValueException(ecoregionName.Value.String,
                                                  "The ecoregion {0} appears more than once.",
                                                  ecoregionName.Value.String);
                ecoregions.Add(ecoregion);
                TextReader.SkipWhitespace(currentLine);
            }
            GetNextLine();

            if(ecoregions.Count == 0)
                throw new InputValueException("", "No ecoregions read in correctly.","");

            return ecoregions;
        }
    }
}
