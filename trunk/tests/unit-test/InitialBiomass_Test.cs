using Edu.Wisc.Forest.Flel.Util;
using Landis.Biomass;
using Landis.Biomass.Succession;
using Landis.Landscape;
using Landis.Species;
using NUnit.Framework;
using System.Collections.Generic;

namespace Landis.Test.Biomass.Succession
{
    [TestFixture]
    public class InitialBiomass_Test
    {
        private ISpecies abiebals;
        private ISpecies betualle;
        private List<ushort> abiebalsAges;
        private List<ushort> betualleAges;
        private List<AgeCohort.ICohort> ageCohorts;
        private Landis.TestUtil.MockCore mockCore;
        private ActiveSite activeSite;
        private MockCalculator mockCalculator;
        private bool isTimestep1;
        private const ushort initialBiomass = 35;
        private const int annualBiomassChange = 2;

        //---------------------------------------------------------------------

        [TestFixtureSetUp]
        public void Init()
        {
            abiebals = Landis.TestUtil.Species.SampleDataset["abiebals"];
            betualle = Landis.TestUtil.Species.SampleDataset["betualle"];
            InitializeAgeCohorts();

            mockCore = new Landis.TestUtil.MockCore();
            mockCore.Landscape = Landis.TestUtil.Landscapes.Create1by1();
            activeSite = mockCore.Landscape[1,1];

            Landis.Biomass.Dead.Pools.Initialize(mockCore, null, null);

            mockCalculator = new MockCalculator();
            mockCalculator.Change = annualBiomassChange;

            Cohort.DeathEvent += DeathNotExpected;
        }

        //---------------------------------------------------------------------

        [TestFixtureTearDown]
        public void TearDown()
        {
            Cohort.DeathEvent -= DeathNotExpected;
        }

        //---------------------------------------------------------------------

        public void DeathNotExpected(object         sender,
                                     DeathEventArgs eventArgs)
        {
            ICohort cohort = eventArgs.Cohort;
            Assert.Fail("A cohort died unexpectedly: {0} age={1} biomass={2}",
                        cohort.Species, cohort.Age, cohort.Biomass);
        }

        //---------------------------------------------------------------------

        private void InitializeAgeCohorts()
        {
            abiebalsAges = new List<ushort>(new ushort[]{30, 40, 50, 150, 170});
            betualleAges = new List<ushort>(new ushort[]{100, 120, 280, 290});

            //  Work with ages from oldest to youngest
            abiebalsAges.Sort(Landis.AgeCohort.Util.WhichIsOlder);
            betualleAges.Sort(Landis.AgeCohort.Util.WhichIsOlder);

            List<AgeCohort.ISpeciesCohorts> speciesCohortList = new List<AgeCohort.ISpeciesCohorts>();
            speciesCohortList.Add(new AgeCohort.SpeciesCohorts(abiebals,
                                                               abiebalsAges));
            speciesCohortList.Add(new AgeCohort.SpeciesCohorts(betualle,
                                                               betualleAges));
            AgeCohort.SiteCohorts siteCohorts = new AgeCohort.SiteCohorts(speciesCohortList);

            ageCohorts = InitialBiomass.SortCohorts(siteCohorts);
        }

        //---------------------------------------------------------------------

        [Test]
        public void MakeCohorts_Timestep10()
        {
            MakeAndCheckCohorts(10);
        }

        //---------------------------------------------------------------------

        [Test]
        public void MakeCohorts_Timestep5()
        {
            MakeAndCheckCohorts(5);
        }

        //---------------------------------------------------------------------

        [Test]
        public void MakeCohorts_Timestep2()
        {
            MakeAndCheckCohorts(2);
        }

        //---------------------------------------------------------------------

        [Test]
        public void MakeCohorts_Timestep1()
        {
            MakeAndCheckCohorts(1);
        }

        //---------------------------------------------------------------------

        private void MakeAndCheckCohorts(int timestep)
        {
            //  All cohort ages are multiples of 10, and this code requires
            //  that the given timestep divides 10 evenly (to mimic binning
            //  process of the initial communities).
            Assert.IsTrue(10 % timestep == 0);

            isTimestep1 = (timestep == 1);
            Landis.Biomass.Cohorts.Initialize(timestep, mockCalculator);
            InitialBiomass.Initialize(timestep);
            SiteCohorts cohorts = InitialBiomass.MakeBiomassCohorts(ageCohorts,
                                                                    activeSite,
                                                                    ComputeInitialBiomass);

            CheckCohorts(cohorts[abiebals], abiebalsAges);
            CheckCohorts(cohorts[betualle], betualleAges);
        }

        //---------------------------------------------------------------------

        public ushort ComputeInitialBiomass(SiteCohorts siteCohorts,
                                            ActiveSite  site)
        {
            return initialBiomass;
        }

        //---------------------------------------------------------------------

        private void CheckCohorts(ISpeciesCohorts biomassCohorts,
                                  List<ushort>    cohortAges)
        {
            Assert.IsNotNull(biomassCohorts);
            Assert.AreEqual(cohortAges.Count, biomassCohorts.Count);

            int i = 0;
            foreach (ICohort cohort in biomassCohorts) {
                ushort age = cohortAges[i];
                Assert.AreEqual(age, cohort.Age);

                //  If the timestep is 1, then annual growth calculations
                //  are called one less time than usual.
                int expectedBiomass;
                if (isTimestep1)
                    expectedBiomass = initialBiomass + (age-1) * annualBiomassChange;
                else
                    expectedBiomass = initialBiomass + age * annualBiomassChange;
                Assert.AreEqual(expectedBiomass, cohort.Biomass);

                i++;
            }
        }
    }
}
