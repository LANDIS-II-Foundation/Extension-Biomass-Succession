using Edu.Wisc.Forest.Flel.Util;
using Landis.Biomass.Succession;
using Landis.Succession;
using NUnit.Framework;
using System.Collections.Generic;

using IEcoregion = Landis.Ecoregions.IEcoregion;
using ISpecies = Landis.Species.ISpecies;

namespace Landis.Test.Biomass.Succession
{
    [TestFixture]
    public class ParametersParser_Test
    {
        private Ecoregions.IDataset ecoregionDataset;
        private Species.IDataset speciesDataset;
        private ParametersParser parser;
        private LineReader reader;
        private InputLine inputLine;

        //---------------------------------------------------------------------

        [TestFixtureSetUp]
        public void Init()
        {
            Ecoregions.DatasetParser ecoregionsParser = new Ecoregions.DatasetParser();
            reader = OpenFile("Ecoregions.txt");
            try {
                ecoregionDataset = ecoregionsParser.Parse(reader);
            }
            finally {
                reader.Close();
            }

            Species.DatasetParser speciesParser = new Species.DatasetParser();
            reader = OpenFile("Species.txt");
            try {
                speciesDataset = speciesParser.Parse(reader);
            }
            finally {
                reader.Close();
            }

            parser = new ParametersParser(ecoregionDataset, speciesDataset);
        }

        //---------------------------------------------------------------------

        private FileLineReader OpenFile(string filename)
        {
            string path = Data.MakeInputPath(filename);
            return Landis.Data.OpenTextFile(path);
        }

        //---------------------------------------------------------------------

        [Test]
        public void GoodFile()
        {
            ReadAndCheckParameters("GoodFile.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        public void GoodFile_AgeOnlyDistParms()
        {
            ReadAndCheckParameters("GoodFile_AgeOnlyDistParms.txt");
        }

        //---------------------------------------------------------------------

        private void ReadAndCheckParameters(string filename)
        {
            IParameters parameters;
            try {
                reader = OpenFile(filename);
                parameters = parser.Parse(reader);
            }
            finally {
                reader.Close();
            }

            try {
                //  Now that we know the data file is properly formatted, read
                //  data from it and compare it against parameter object.
                reader = OpenFile(filename);
                inputLine = new InputLine(reader);

                Assert.AreEqual(parser.LandisDataValue, ReadInputVar<string>("LandisData"));

                Assert.AreEqual(ReadInputVar<int>("Timestep"), parameters.Timestep);
                Assert.AreEqual(ReadInputVar<SeedingAlgorithms>("SeedingAlgorithm"),
                                parameters.SeedAlgorithm);

                inputLine.MatchName("MinRelativeBiomass");
                inputLine.GetNext();
                List<IEcoregion> ecoregions = ReadEcoregions();
                for (byte shadeClass = 1; shadeClass <= 5; shadeClass++) {
                    StringReader currentLine = new StringReader(inputLine.ToString());
                    Assert.AreEqual(shadeClass, ReadInputValue<byte>(currentLine));
                    foreach (IEcoregion ecoregion in ecoregions)
                        //  TODO: Eventually allow equality testing for Percentage
                        Assert.AreEqual((double) ReadInputValue<Percentage>(currentLine),
                                        (double) parameters.MinRelativeBiomass[shadeClass][ecoregion]);
                    inputLine.GetNext();
                }

                inputLine.MatchName("BiomassParameters");
                inputLine.GetNext();
                while (inputLine.VariableName != "EstablishProbabilities") {
                    StringReader currentLine = new StringReader(inputLine.ToString());
                    ISpecies species = ReadSpecies(currentLine);
                    Assert.AreEqual(ReadInputValue<double>(currentLine),
                                    parameters.LeafLongevity[species]);
                    Assert.AreEqual(ReadInputValue<double>(currentLine),
                                    parameters.WoodyDecayRate[species]);
                    Assert.AreEqual(ReadInputValue<double>(currentLine),
                                    parameters.MortCurveShapeParm[species]);
                    inputLine.GetNext();
                }

                CheckParameterTable("EstablishProbabilities",
                                    parameters.EstablishProbability,
                                    "MaxANPP");

                CheckParameterTable("MaxANPP",
                                    parameters.MaxANPP,
                                    "LeafLitter:DecayRates");

                const string AgeOnlyDisturbanceParms = "AgeOnlyDisturbances:BiomassParameters";
                CheckParameterTable("LeafLitter:DecayRates",
                                    parameters.LeafLitterDecayRate,
                                    AgeOnlyDisturbanceParms);

                if (parameters.AgeOnlyDisturbanceParms != null)
                    Assert.AreEqual(ReadInputVar<string>(AgeOnlyDisturbanceParms),
                                    parameters.AgeOnlyDisturbanceParms);
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

        private List<IEcoregion> ReadEcoregions()
        {
            List<IEcoregion> ecoregions = new List<IEcoregion>();

            InputVar<string> ecoregionName = new InputVar<string>("Ecoregion");

            StringReader currentLine = new StringReader(inputLine.ToString());
            TextReader.SkipWhitespace(currentLine);
            while (currentLine.Peek() != -1) {
                ecoregionName.ReadValue(currentLine);
                IEcoregion ecoregion = ecoregionDataset[ecoregionName.Value.Actual];
                Assert.IsNotNull(ecoregion);
                ecoregions.Add(ecoregion);
                TextReader.SkipWhitespace(currentLine);
            }
            inputLine.GetNext();
            return ecoregions;
        }

        //---------------------------------------------------------------------

        private ISpecies ReadSpecies(StringReader currentLine)
        {
            InputVar<string> speciesName = new InputVar<string>("Species");
            speciesName.ReadValue(currentLine);
            ISpecies species = speciesDataset[speciesName.Value.Actual];
            Assert.IsNotNull(species);
            return species;
        }

        //---------------------------------------------------------------------

        private void CheckParameterTable<TParm>(string                                     tableName,
                                                Species.AuxParm<Ecoregions.AuxParm<TParm>> parmValues,
                                                string                                     nextTableName)
        {
            inputLine.MatchName(tableName);
            bool haveLine = inputLine.GetNext();

            List<IEcoregion> ecoregions = ReadEcoregions();
            while (haveLine && inputLine.VariableName != nextTableName) {
                StringReader currentLine = new StringReader(inputLine.ToString());
                ISpecies species = ReadSpecies(currentLine);
                foreach (IEcoregion ecoregion in ecoregions)
                    Assert.AreEqual(ReadInputValue<TParm>(currentLine),
                                    parmValues[species][ecoregion]);
                haveLine = inputLine.GetNext();
            }
        }

        //---------------------------------------------------------------------

        private void TryParse(string filename)
        {
            int? errorLineNum = Testing.FindErrorMarker(Data.MakeInputPath(filename));
            try {
                reader = OpenFile(filename);
                IParameters parameters = parser.Parse(reader);
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
        public void Timestep_Missing()
        {
            TryParse("Timestep-Missing.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void Timestep_Negative()
        {
            TryParse("Timestep-Negative.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void SeedAlg_Missing()
        {
            TryParse("SeedAlg-Missing.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void SeedAlg_Unknown()
        {
            TryParse("SeedAlg-Unknown.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void MinRelBiomass_Missing()
        {
            TryParse("MinRelBiomass-Missing.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void MinRelBiomass_MissingEcoregions()
        {
            TryParse("MinRelBiomass-MissingEcoregions.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void MinRelBiomass_UnknownEcoregion()
        {
            TryParse("MinRelBiomass-UnknownEcoregion.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void MinRelBiomass_InactiveEcoregion()
        {
            TryParse("MinRelBiomass-InactiveEcoregion.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void MinRelBiomass_RepeatedEcoregion()
        {
            TryParse("MinRelBiomass-RepeatedEcoregion.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void MinRelBiomass_MissingRows()
        {
            TryParse("MinRelBiomass-MissingRows.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void MinRelBiomass_FirstRowNotClass1()
        {
            TryParse("MinRelBiomass-FirstRowNotClass1.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void MinRelBiomass_MissingValue()
        {
            TryParse("MinRelBiomass-MissingValue.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void MinRelBiomass_ExtraText()
        {
            TryParse("MinRelBiomass-ExtraText.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void MinRelBiomass_BadValue()
        {
            TryParse("MinRelBiomass-BadValue.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void MinRelBiomass_BelowMin()
        {
            TryParse("MinRelBiomass-BelowMin.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void MinRelBiomass_AboveMax()
        {
            TryParse("MinRelBiomass-AboveMax.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void MinRelBiomass_ExtraRow()
        {
            TryParse("MinRelBiomass-ExtraRow.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void MinRelBiomass_MissingRow4()
        {
            TryParse("MinRelBiomass-MissingRow4.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void BiomassParms_Missing()
        {
            TryParse("BiomassParms-Missing.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void BiomassParms_WrongName()
        {
            TryParse("BiomassParms-WrongName.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void BiomassParms_UnknownSpecies()
        {
            TryParse("BiomassParms-UnknownSpecies.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void BiomassParms_RepeatedSpecies()
        {
            TryParse("BiomassParms-RepeatedSpecies.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void BiomassParms_ExtraText()
        {
            TryParse("BiomassParms-ExtraText.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void LeafLongevity_Missing()
        {
            TryParse("LeafLongevity-Missing.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void LeafLongevity_BadValue()
        {
            TryParse("LeafLongevity-BadValue.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void LeafLongevity_BelowMin()
        {
            TryParse("LeafLongevity-BelowMin.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void LeafLongevity_AboveMax()
        {
            TryParse("LeafLongevity-AboveMax.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void WoodyDecayRate_Missing()
        {
            TryParse("WoodyDecayRate-Missing.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void WoodyDecayRate_BadValue()
        {
            TryParse("WoodyDecayRate-BadValue.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void WoodyDecayRate_BelowMin()
        {
            TryParse("WoodyDecayRate-BelowMin.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void WoodyDecayRate_AboveMax()
        {
            TryParse("WoodyDecayRate-AboveMax.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void MortCurveShapeParm_Missing()
        {
            TryParse("MortCurveShapeParm-Missing.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void MortCurveShapeParm_BadValue()
        {
            TryParse("MortCurveShapeParm-BadValue.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void MortCurveShapeParm_BelowMin()
        {
            TryParse("MortCurveShapeParm-BelowMin.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void MortCurveShapeParm_AboveMax()
        {
            TryParse("MortCurveShapeParm-AboveMax.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void EstablishProb_Missing()
        {
            TryParse("EstablishProb-Missing.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void EstablishProb_UnknownEcoregion()
        {
            TryParse("EstablishProb-UnknownEcoregion.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void EstablishProb_InactiveEcoregion()
        {
            TryParse("EstablishProb-InactiveEcoregion.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void EstablishProb_RepeatedEcoregion()
        {
            TryParse("EstablishProb-RepeatedEcoregion.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void EstablishProb_UnknownSpecies()
        {
            TryParse("EstablishProb-UnknownSpecies.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void EstablishProb_RepeatedSpecies()
        {
            TryParse("EstablishProb-RepeatedSpecies.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void EstablishProb_MissingValue()
        {
            TryParse("EstablishProb-MissingValue.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void EstablishProb_BadValue()
        {
            TryParse("EstablishProb-BadValue.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void EstablishProb_BelowMin()
        {
            TryParse("EstablishProb-BelowMin.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void EstablishProb_AboveMax()
        {
            TryParse("EstablishProb-AboveMax.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void EstablishProb_ExtraText()
        {
            TryParse("EstablishProb-ExtraText.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void MaxANPP_Missing()
        {
            TryParse("MaxANPP-Missing.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void MaxANPP_UnknownEcoregion()
        {
            TryParse("MaxANPP-UnknownEcoregion.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void MaxANPP_InactiveEcoregion()
        {
            TryParse("MaxANPP-InactiveEcoregion.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void MaxANPP_RepeatedEcoregion()
        {
            TryParse("MaxANPP-RepeatedEcoregion.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void MaxANPP_UnknownSpecies()
        {
            TryParse("MaxANPP-UnknownSpecies.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void MaxANPP_RepeatedSpecies()
        {
            TryParse("MaxANPP-RepeatedSpecies.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void MaxANPP_MissingValue()
        {
            TryParse("MaxANPP-MissingValue.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void MaxANPP_BadValue()
        {
            TryParse("MaxANPP-BadValue.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void MaxANPP_BelowMin()
        {
            TryParse("MaxANPP-BelowMin.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void MaxANPP_AboveMax()
        {
            TryParse("MaxANPP-AboveMax.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void MaxANPP_ExtraText()
        {
            TryParse("MaxANPP-ExtraText.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void LeafDecayRate_Missing()
        {
            TryParse("LeafDecayRate-Missing.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void LeafDecayRate_UnknownEcoregion()
        {
            TryParse("LeafDecayRate-UnknownEcoregion.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void LeafDecayRate_InactiveEcoregion()
        {
            TryParse("LeafDecayRate-InactiveEcoregion.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void LeafDecayRate_RepeatedEcoregion()
        {
            TryParse("LeafDecayRate-RepeatedEcoregion.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void LeafDecayRate_UnknownSpecies()
        {
            TryParse("LeafDecayRate-UnknownSpecies.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void LeafDecayRate_RepeatedSpecies()
        {
            TryParse("LeafDecayRate-RepeatedSpecies.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void LeafDecayRate_MissingValue()
        {
            TryParse("LeafDecayRate-MissingValue.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void LeafDecayRate_BadValue()
        {
            TryParse("LeafDecayRate-BadValue.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void LeafDecayRate_BelowMin()
        {
            TryParse("LeafDecayRate-BelowMin.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void LeafDecayRate_AboveMax()
        {
            TryParse("LeafDecayRate-AboveMax.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void LeafDecayRate_ExtraText()
        {
            TryParse("LeafDecayRate-ExtraText.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void AgeOnlyDistParms_Empty()
        {
            TryParse("AgeOnlyDistParms-Empty.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void AgeOnlyDistParms_Whitespace()
        {
            TryParse("AgeOnlyDistParms-Whitespace.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void ExtraDataAfterLastParm()
        {
            TryParse("ExtraDataAfterLastParm.txt");
        }
    }
}
