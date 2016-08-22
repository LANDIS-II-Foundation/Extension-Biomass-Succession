using Edu.Wisc.Forest.Flel.Util;
using Landis.Biomass.Succession.AgeOnlyDisturbances;
using Landis.PlugIns;
using NUnit.Framework;

namespace Landis.Test.Biomass.Succession.AgeOnlyDisturbances
{
    [TestFixture]
    public class DatasetParser_Test
    {
        private DatasetParser parser;
        private LineReader reader;
        private InputLine inputLine;

        //---------------------------------------------------------------------

        [TestFixtureSetUp]
        public void Init()
        {
            parser = new DatasetParser();

            //  FIXME: A hack to ensure that Percentage has a registered Parse
            //  method for reading input values
            double d = Percentage.MaxValue;
        }

        //---------------------------------------------------------------------

        private string MakeInputPath(string filename)
        {
            return Data.MakeInputPath(System.IO.Path.Combine("age-only-disturbances", filename));
        }

        //---------------------------------------------------------------------

        private FileLineReader OpenFile(string filename)
        {
            return Landis.Data.OpenTextFile(MakeInputPath(filename));
        }

        //---------------------------------------------------------------------

        [Test]
        public void GoodFile()
        {
            const string filename = "GoodFile.txt";
            IParameterDataset dataset;
            try {
                reader = OpenFile(filename);
                dataset = parser.Parse(reader);
            }
            finally {
                reader.Close();
            }


            try {
                //  Now that we know the data file is properly formatted, read
                //  data from it and compare it against parameter dataset.
                reader = OpenFile(filename);
                inputLine = new InputLine(reader);

                Assert.AreEqual(parser.LandisDataValue, ReadInputVar<string>("LandisData"));

                CheckPercentageTable("CohortBiomassReductions",
                                     dataset.CohortReductions,
                                     "DeadPoolReductions");

                CheckPercentageTable("DeadPoolReductions",
                                     dataset.PoolReductions,
                                     null);
            }
            finally {
                inputLine = null;
                reader.Close();
            }
        }

        //---------------------------------------------------------------------

        private TValue ReadInputVar<TValue>(string name)
        {
            InputVar<TValue> var = new InputVar<TValue>(name);
            inputLine.ReadVar(var);
            inputLine.GetNext();
            return var.Value.Actual;
        }

        //---------------------------------------------------------------------

        private TValue ReadInputValue<TValue>(StringReader currentLine)
        {
            InputVar<TValue> var = new InputVar<TValue>("(dummy)");
            var.ReadValue(currentLine);
            return var.Value.Actual;
        }

        //---------------------------------------------------------------------

        private void CheckPercentageTable(string           tableName,
                                          IPercentageTable table,
                                          string           nextTableName)
        {
            inputLine.MatchName(tableName);
            bool haveLine = inputLine.GetNext();

            while (haveLine && inputLine.VariableName != nextTableName) {
                StringReader currentLine = new StringReader(inputLine.ToString());
                string disturbance = ReadInputValue<string>(currentLine);

                PlugInType disturbanceType;
                if (disturbance == "(default)")
                    disturbanceType = new PlugInType(disturbance);
                else
                    disturbanceType = new PlugInType("disturbance:" + disturbance);

                PoolPercentages percentages = table[disturbanceType];
                Assert.AreEqual((double) percentages.Woody,
                                (double) ReadInputValue<Percentage>(currentLine));
                Assert.AreEqual((double) percentages.NonWoody,
                                (double) ReadInputValue<Percentage>(currentLine));

                haveLine = inputLine.GetNext();
            }
        }

        //---------------------------------------------------------------------

        private void TryParse(string filename)
        {
            int? errorLineNum = Testing.FindErrorMarker(MakeInputPath(filename));
            try {
                reader = OpenFile(filename);
                IParameterDataset dataset = parser.Parse(reader);
            }
            catch (System.Exception e) {
                Data.Output.WriteLine();
                Data.Output.WriteLine(e.Message.Replace(Data.Directory, Data.DirPlaceholder));
                LineReaderException lrExc = e as LineReaderException;
                if (lrExc != null && errorLineNum.HasValue)
                    Assert.AreEqual(errorLineNum.Value, lrExc.LineNumber);
                throw;
            }
            finally {
                reader.Close();
            }
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void LandisData_WrongValue()
        {
            TryParse("LandisData-WrongValue.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void CohortReduction_Missing()
        {
            TryParse("CohortReduction-Missing.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void CohortReduction_WrongName()
        {
            TryParse("CohortReduction-WrongName.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void CohortReduction_NoEntries()
        {
            TryParse("CohortReduction-NoEntries.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void CohortReduction_Woody_Missing()
        {
            TryParse("CohortReduction-Woody-Missing.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void CohortReduction_Woody_BadValue()
        {
            TryParse("CohortReduction-Woody-BadValue.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void CohortReduction_Woody_BelowMin()
        {
            TryParse("CohortReduction-Woody-BelowMin.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void CohortReduction_Woody_AboveMax()
        {
            TryParse("CohortReduction-Woody-AboveMax.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void CohortReduction_NonWoody_Missing()
        {
            TryParse("CohortReduction-NonWoody-Missing.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void CohortReduction_NonWoody_BadValue()
        {
            TryParse("CohortReduction-NonWoody-BadValue.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void CohortReduction_NonWoody_BelowMin()
        {
            TryParse("CohortReduction-NonWoody-BelowMin.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void CohortReduction_NonWoody_AboveMax()
        {
            TryParse("CohortReduction-NonWoody-AboveMax.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void CohortReduction_ExtraText()
        {
            TryParse("CohortReduction-ExtraText.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void CohortReduction_RepeatedDisturbance()
        {
            TryParse("CohortReduction-RepeatedDisturbance.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void CohortReduction_NoDefault()
        {
            TryParse("CohortReduction-NoDefault.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void CohortReduction_RepeatedDefault()
        {
            TryParse("CohortReduction-RepeatedDefault.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void PoolReduction_Missing()
        {
            TryParse("PoolReduction-Missing.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void PoolReduction_NoEntries()
        {
            TryParse("PoolReduction-NoEntries.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void PoolReduction_Woody_Missing()
        {
            TryParse("PoolReduction-Woody-Missing.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void PoolReduction_Woody_BadValue()
        {
            TryParse("PoolReduction-Woody-BadValue.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void PoolReduction_Woody_BelowMin()
        {
            TryParse("PoolReduction-Woody-BelowMin.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void PoolReduction_Woody_AboveMax()
        {
            TryParse("PoolReduction-Woody-AboveMax.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void PoolReduction_NonWoody_Missing()
        {
            TryParse("PoolReduction-NonWoody-Missing.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void PoolReduction_NonWoody_BadValue()
        {
            TryParse("PoolReduction-NonWoody-BadValue.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void PoolReduction_NonWoody_BelowMin()
        {
            TryParse("PoolReduction-NonWoody-BelowMin.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void PoolReduction_NonWoody_AboveMax()
        {
            TryParse("PoolReduction-NonWoody-AboveMax.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void PoolReduction_ExtraText()
        {
            TryParse("PoolReduction-ExtraText.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void PoolReduction_RepeatedDisturbance()
        {
            TryParse("PoolReduction-RepeatedDisturbance.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void PoolReduction_NoDefault()
        {
            TryParse("PoolReduction-NoDefault.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void PoolReduction_RepeatedDefault()
        {
            TryParse("PoolReduction-RepeatedDefault.txt");
        }
    }
}
