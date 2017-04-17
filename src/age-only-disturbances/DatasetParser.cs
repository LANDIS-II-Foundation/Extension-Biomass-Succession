//  Copyright 2005-2010 Portland State University, University of Wisconsin
//  Authors:  Robert M. Scheller, James B. Domingo

using Landis.Core;
using System.Collections.Generic;
using Landis.Utilities;

namespace Landis.Extension.Succession.Biomass.AgeOnlyDisturbances
{
    /// <summary>
    /// A parser that reads a dataset of biomass parameters for age-only
    /// disturbances from text input.
    /// </summary>
    public class DatasetParser
        : Landis.Utilities.TextParser<IParameterDataset>
    {
        private Dictionary<string, int> lineNums;

        //---------------------------------------------------------------------
      
        static DatasetParser()
        {
            //    FIXME: Need to add RegisterForInputValues method to
            //  Percentage class, but for now, we'll trigger it by creating
            //  a local variable of that type.
            Percentage dummy = new Percentage();
        }
        //---------------------------------------------------------------------

        public DatasetParser()
        {
            lineNums = new Dictionary<string, int>();
        }

        //---------------------------------------------------------------------

        protected override IParameterDataset Parse()
        {
            InputVar<string> landisData = new InputVar<string>("LandisData");
            ReadVar(landisData);
            if (landisData.Value.Actual != "Age-only Disturbances - Biomass Parameters")
                throw new InputValueException(landisData.Value.String, "The value is not \"{0}\"", "Age-only Disturbances - Biomass Parameters");

            ParameterDataset dataset = new ParameterDataset();
            const string DeadBiomassReductions = "DeadBiomassReductions";

            ParseTable(dataset.CohortReductions,
                       "CohortBiomassReductions",
                       DeadBiomassReductions);

            ParseTable(dataset.PoolReductions,
                       DeadBiomassReductions,
                       null);

            return dataset; 
        }

        //---------------------------------------------------------------------

        private void ParseTable(IPercentageTable table,
                                string                  tableName,
                                string                  nextTableName)
        {
            ReadName(tableName);

            PlugIn.ModelCore.UI.WriteLine("   Age-only-disturbances:  reading {0}.", tableName);

            InputVar<string> disturbance = new InputVar<string>("Disturbance");
            InputVar<Percentage> woodPercentage = new InputVar<Percentage>("Woody");
            InputVar<Percentage> foliarPercentage = new InputVar<Percentage>("Non-Woody");
            string lastColumn = "the " + foliarPercentage.Name + " column";

            const string defaultName = "(default)";
            bool defaultFound = false;
            lineNums.Clear();
            while (! AtEndOfInput && CurrentName != nextTableName) {
                StringReader currentLine = new StringReader(CurrentLine);

                ReadValue(disturbance, currentLine);
                int lineNum;
                if (lineNums.TryGetValue(disturbance.Value.Actual, out lineNum))
                    throw new InputValueException(disturbance.Value.Actual,
                                                  "The value \"{0}\" was previously used on line {1}",
                                                  disturbance.Value.Actual,
                                                  lineNum);
                lineNums[disturbance.Value.String] = LineNumber;

                PoolPercentages percentages;
                if (disturbance.Value.Actual == defaultName) {
                    defaultFound = true;
                    percentages = table.Default;
                }
                else {
                    ExtensionType disturbanceType = new ExtensionType("disturbance:" + disturbance.Value.Actual);

                    if(disturbance.Value.Actual == "Fire")
                        throw new InputValueException(disturbance.Value.Actual,
                                                  "\"{0}\" is not an allowable disturbance type, line {1}",
                                                  disturbance.Value.Actual,
                                                  lineNum);

                    percentages = table[disturbanceType];
                }

                ReadValue(woodPercentage, currentLine);
                percentages.Wood = woodPercentage.Value;

                ReadValue(foliarPercentage, currentLine);
                percentages.Foliar = foliarPercentage.Value;

                CheckNoDataAfter(lastColumn, currentLine);
                GetNextLine();
            }

            if (! defaultFound)
                throw NewParseException("Missing the \"{0}\" row in the percentage table",
                                        defaultName);
        }
    }
}
