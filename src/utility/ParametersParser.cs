//  Copyright 2005-2010 Portland State University
//  Authors:  Robert M. Scheller
//  License:  Available at
//  http://www.landis-ii.org/developers/LANDIS-IISourceCodeLicenseAgreement.pdf

using Edu.Wisc.Forest.Flel.Util;

using Landis.Ecoregions;
using Landis.Species;
using Landis.Succession;

using System.Collections.Generic;

namespace Landis.Biomass.Succession
{
    /// <summary>
    /// A parser that reads biomass succession parameters from text input.
    /// </summary>
    public class ParametersParser
        : ClimateChange.BiomassParametersParser<IParameters>
    {
        public static class Names
        {
            public const string Timestep = "Timestep";
            public const string SeedingAlgorithm = "SeedingAlgorithm";
            public const string AgeOnlyDisturbanceParms = "AgeOnlyDisturbances:BiomassParameters";
            public const string ClimateChange = "ClimateChange";
            public const string CalibrateMode = "CalibrateMode";
        }

        //---------------------------------------------------------------------

        private Ecoregions.IDataset ecoregionDataset;
        private Species.IDataset speciesDataset;

        //---------------------------------------------------------------------

        public override string LandisDataValue
        {
            get {
                return "Biomass Succession v2";
            }
        }

        //---------------------------------------------------------------------

        static ParametersParser()
        {
            SeedingAlgorithmsUtil.RegisterForInputValues();
        }

        //---------------------------------------------------------------------

        public ParametersParser(Ecoregions.IDataset ecoregionDataset,
                                Species.IDataset    speciesDataset,
                                int                 startYear,
                                int                 endYear)
            : base(ecoregionDataset,
                   speciesDataset)
        {
            this.ecoregionDataset = ecoregionDataset;
            this.speciesDataset = speciesDataset;

            ClimateChange.InputValidation.Initialize(startYear, endYear);
        }

        //---------------------------------------------------------------------

        protected override IParameters Parse()
        {
            ReadLandisDataVar();

            Parameters parameters = new Parameters(ecoregionDataset, speciesDataset);

            InputVar<int> timestep = new InputVar<int>(Names.Timestep);
            ReadVar(timestep);
            parameters.Timestep = timestep.Value;

            InputVar<SeedingAlgorithms> seedAlg = new InputVar<SeedingAlgorithms>(Names.SeedingAlgorithm);
            ReadVar(seedAlg);
            parameters.SeedAlgorithm = seedAlg.Value;

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

            ParseBiomassParameters(parameters, Names.AgeOnlyDisturbanceParms,
                                               Names.ClimateChange);

            //  AgeOnlyDisturbances:BiomassParameters (optional)
            string lastParameter = null;
            if (! AtEndOfInput && CurrentName == Names.AgeOnlyDisturbanceParms) {
                InputVar<string> ageOnlyDisturbanceParms = new InputVar<string>(Names.AgeOnlyDisturbanceParms);
                ReadVar(ageOnlyDisturbanceParms);
                parameters.AgeOnlyDisturbanceParms = ageOnlyDisturbanceParms.Value;

                lastParameter = "the " + Names.AgeOnlyDisturbanceParms + " parameter";
            }

            //  Climate Change table (optional)
            if (ReadOptionalName(Names.ClimateChange)) {
                ReadClimateChangeTable(parameters.ClimateChangeUpdates);
            }
            else if (lastParameter != null)
                CheckNoDataAfter(lastParameter);

            return parameters; //.GetComplete();
        }

        //---------------------------------------------------------------------

        protected void ReadClimateChangeTable(List<ClimateChange.ParametersUpdate> parameterUpdates)
        {
            int? prevYear = null;
            int prevYearLineNum = 0;
            InputVar<int> year = new InputVar<int>("Year", ClimateChange.InputValidation.ReadYear);
            InputVar<string> file = new InputVar<string>("Parameter File");
            while (! AtEndOfInput) {
                StringReader currentLine = new StringReader(CurrentLine);

                ReadValue(year, currentLine);
                if (prevYear.HasValue) {
                    if (year.Value.Actual < prevYear.Value)
                        throw new InputValueException(year.Value.String,
                                                      "Year {0} is before year {1} which was on line {2}",
                                                      year.Value.Actual, prevYear.Value, prevYearLineNum);
                    if (year.Value.Actual == prevYear.Value)
                        throw new InputValueException(year.Value.String,
                                                      "Year {0} was already used on line {1}",
                                                      year.Value.Actual, prevYearLineNum);
                }
                prevYear = year.Value.Actual;
                prevYearLineNum = LineNumber;

                ReadValue(file, currentLine);
                ClimateChange.InputValidation.CheckPath(file.Value);

                CheckNoDataAfter("the " + file + " column", currentLine);
                parameterUpdates.Add(new ClimateChange.ParametersUpdate(year.Value.Actual,
                                                                        file.Value.Actual));
                GetNextLine();
            }
        }
    }
}
