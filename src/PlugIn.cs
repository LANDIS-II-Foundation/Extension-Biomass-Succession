//  Authors:  Robert M. Scheller

using Landis.Core;
using Landis.SpatialModeling;
using Landis.Library.Climate;
using Landis.Library.Succession;
using Landis.Library.InitialCommunities.Universal;
using Landis.Library.UniversalCohorts;
using System.Collections.Generic;
using System.Linq;
using System;
using Landis.Library.Metadata;

namespace Landis.Extension.Succession.Biomass
{
    public class PlugIn
        : Landis.Library.Succession.ExtensionBase
    {
        public static readonly string ExtensionName = "Biomass Succession";
        public static readonly string summaryLogFileName = "Biomass-succession-log.csv";
        private static ICore modelCore;
        private List<ISufficientLight> sufficientLight;
        public static bool CalibrateMode;
        public static double CurrentYearSiteMortality;
        private static int time;
        public static int FutureClimateBaseYear;
        public static MetadataTable<SummaryLog> summaryLog;
        public static IInputParameters Parameters;
        private ICommunity initialCommunity;


        //---------------------------------------------------------------------

        public PlugIn()
            : base(ExtensionName)
        {
        }

        //---------------------------------------------------------------------

        public override void LoadParameters(string dataFile, ICore mCore)
        {
            modelCore = mCore;
            InputParametersParser parser = new InputParametersParser();
            Parameters = Landis.Data.Load<IInputParameters>(dataFile, parser);
        }

        //---------------------------------------------------------------------

        public static ICore ModelCore
        {
            get
            {
                return modelCore;
            }
        }
        //---------------------------------------------------------------------

        public static int SuccessionTimeStep
        {
            get
            {
                return time;
            }
        }

        //---------------------------------------------------------------------

        public override void Initialize()
        {

            Timestep = Parameters.Timestep;
            time = Timestep;

            CalibrateMode = Parameters.CalibrateMode;
            //CohortBiomass.SpinupMortalityFraction = Parameters.SpinupMortalityFraction;

            //Initialize climate.
            if (Parameters.ClimateConfigFile != null)
            {
                Climate.Initialize(Parameters.ClimateConfigFile, false, modelCore);
                ClimateRegionData.Initialize(Parameters);
            }

            sufficientLight = Parameters.LightClassProbabilities;

            SpeciesData.Initialize(Parameters);
            FireEffects.Initialize(Parameters);
            SiteVars.Initialize();
            EcoregionData.Initialize(Parameters);
            SpeciesData.GetAnnualData(0);  // Year 0

            MetadataHandler.InitializeMetadata(summaryLogFileName);
            
            //  Cohorts must be created before the base class is initialized
            //  because the base class' reproduction module uses the core's
            //  SuccessionCohorts property in its Initialization method.
            Landis.Library.UniversalCohorts.Cohorts.Initialize(Timestep, new CohortBiomass());

            // Initialize Reproduction routines:
            Reproduction.SufficientResources = SufficientLight;
            Reproduction.Establish = Establish;
            Reproduction.AddNewCohort = AddNewCohort;
            Reproduction.MaturePresent = MaturePresent;
            Reproduction.PlantingEstablish = PlantingEstablish;
            base.Initialize(modelCore, Parameters.SeedAlgorithm); 

            InitialBiomass.Initialize(Timestep);
            Cohort.MortalityEvent += CohortMortality;

            InitializeSites(Parameters.InitialCommunities, Parameters.InitialCommunitiesMap, modelCore);
        }


        //---------------------------------------------------------------------

        protected override void InitializeSite(ActiveSite site)//,ICommunity initialCommunity)
        {
            InitialBiomass initialBiomass = InitialBiomass.Compute(site, initialCommunity);
            //SiteVars.Cohorts[site] = InitialBiomass.Clone(initialBiomass.Cohorts); 
            //SiteVars.WoodyDebris[site] = initialBiomass.DeadWoodyPool.Clone();
            //SiteVars.Litter[site] = initialBiomass.DeadNonWoodyPool.Clone();
        }

        //---------------------------------------------------------------------

        public override void Run()
        {
            //if(PlugIn.ModelCore.CurrentTime == Timestep)
                //Outputs.WriteLogFile(0);

            if(PlugIn.ModelCore.CurrentTime > 0 && SiteVars.HarvestCapacityReduction == null)
                SiteVars.HarvestCapacityReduction   = PlugIn.ModelCore.GetSiteVar<double>("Harvest.CapacityReduction");

            base.Run();

            if (Timestep > 0 && Parameters.ClimateConfigFile != null)
                ClimateRegionData.SetAllEcoregions_FutureAnnualClimate(ModelCore.CurrentTime);

            Outputs.WriteLogFile(PlugIn.ModelCore.CurrentTime);

            // Reset establishment modifier to 1.0 after each time step
            foreach (ISpecies species in PlugIn.ModelCore.Species)
            {
                foreach (IEcoregion ecoregion in PlugIn.ModelCore.Ecoregions)
                {
                    if (!ecoregion.Active)
                        continue;
                    SpeciesData.EstablishModifier[species, ecoregion] = 1.0;
                }
            }
        }


        //---------------------------------------------------------------------
        // Revised 10/5/09 - BRM

        public override byte ComputeShade(ActiveSite site)
        {
            IEcoregion ecoregion = ModelCore.Ecoregion[site];
            double B_ACT = 0.0;

            if (SiteVars.Cohorts[site] != null)
            {
                foreach (ISpeciesCohorts sppCohorts in SiteVars.Cohorts[site])
                    foreach (ICohort cohort in sppCohorts)
                        if (cohort.Data.Age > 5)
                            B_ACT += cohort.Data.Biomass;
            }

            int lastMortality = SiteVars.PreviousYearMortality[site];
            B_ACT = System.Math.Min(EcoregionData.B_MAX[ecoregion] - lastMortality, B_ACT);


            //  Relative living biomass (ratio of actual to maximum site
            //  biomass).
            double B_AM = B_ACT / EcoregionData.B_MAX[ecoregion];

            for (byte shade = 5; shade >= 1; shade--)
            {
                if (EcoregionData.MinRelativeBiomass[shade][ecoregion] == null)
                {
                    string mesg = string.Format("Minimum relative biomass has not been defined for ecoregion {0}", ecoregion.Name);
                    throw new System.ApplicationException(mesg);
                }
                if (B_AM >= EcoregionData.MinRelativeBiomass[shade][ecoregion])
                    return shade;
            }
            return 0;

        }
        //---------------------------------------------------------------------

        public void CohortMortality(object sender, MortalityEventArgs eventArgs)
        {
            ExtensionType disturbanceType = eventArgs.DisturbanceType;
            ActiveSite site = eventArgs.Site;

            ICohort cohort = eventArgs.Cohort;

            double partialMortality = (double)eventArgs.Reduction;
            if (PlugIn.CalibrateMode && PlugIn.ModelCore.CurrentTime > 0)
            {
                PlugIn.ModelCore.UI.WriteLine("   BIOMASS SUCCESSION MORTALITY I: species={0}, age={1}, disturbance={2}.", cohort.Species.Name, cohort.Data.Age, disturbanceType);
                PlugIn.ModelCore.UI.WriteLine("   BIOMASS SUCCESSION MORTALITY I: eventReduction={0:0.0}, new_cohort_biomass={1}.", eventArgs.Reduction, cohort.Data.Biomass);
            }
            double nonWoodyFraction = (double) cohort.ComputeNonWoodyBiomass(site) / (double) cohort.Data.Biomass;
            double woodyFraction = 1.0 - nonWoodyFraction;

            float foliarInput = (float) (partialMortality * nonWoodyFraction); 
            float woodInput = (float)(partialMortality * woodyFraction); 

            if (disturbanceType != null)
            {
                if (PlugIn.CalibrateMode && PlugIn.ModelCore.CurrentTime > 0)
                    PlugIn.ModelCore.UI.WriteLine("   BIOMASS SUCCESSION MORTALITY II: species={0}, age={1}, woodInput={2}, foliarInputs={3}.", cohort.Species.Name, cohort.Data.Age, woodInput, foliarInput);

                if (disturbanceType.IsMemberOf("disturbance:harvest"))
                {
                    SiteVars.HarvestPrescriptionName = PlugIn.ModelCore.GetSiteVar<string>("Harvest.PrescriptionName");
                    if (!Disturbed[site]) // this is the first cohort killed/damaged
                    {
                        HarvestEffects.ReduceLayers(site);
                    }
                    woodInput -= woodInput * (float)HarvestEffects.GetCohortWoodRemoval(site);
                    foliarInput -= foliarInput * (float)HarvestEffects.GetCohortLeafRemoval(site);
                }
                //PlugIn.ModelCore.UI.WriteLine("   BIOMASS SUCCESSION PARTIAL MORTALITY: species={0}, age={1}, woodInput={2}, foliarInputs={3}.", cohort.Species.Name, cohort.Data.Age, woodInput, foliarInput);
                if (disturbanceType.IsMemberOf("disturbance:fire"))
                {
                    SiteVars.FireSeverity = PlugIn.ModelCore.GetSiteVar<byte>("Fire.Severity");

                    if (eventArgs.Reduction >= 1)
                    {
                        Landis.Library.Succession.Reproduction.CheckForPostFireRegen(eventArgs.Cohort, site);
                    }

                    if (!Disturbed[site]) // this is the first cohort killed/damaged
                    {
                        if (SiteVars.FireSeverity != null && SiteVars.FireSeverity[site] > 0)
                            FireEffects.ReduceLayers(SiteVars.FireSeverity[site], site);
                    }

                    double woodFireConsumption = woodInput * (float)FireEffects.ReductionsTable[(int)SiteVars.FireSeverity[site]].CoarseLitterReduction;
                    double foliarFireConsumption = foliarInput * (float)FireEffects.ReductionsTable[(int)SiteVars.FireSeverity[site]].FineLitterReduction;

                    woodInput -= (float)woodFireConsumption;
                    foliarInput -= (float)foliarFireConsumption;
                }
                else
                {
                    // If not fire, check for resprouting:
                    Landis.Library.Succession.Reproduction.CheckForResprouting(eventArgs.Cohort, site);
                }

                if (PlugIn.CalibrateMode && PlugIn.ModelCore.CurrentTime > 0)
                    PlugIn.ModelCore.UI.WriteLine("   BIOMASS SUCCESSION MORTALITY III: ForestFloorInputs: Foliar={0:0.00}, Wood={1:0.0}.", foliarInput, woodInput);
            }

            ForestFloor.AddWoody(woodInput, cohort.Species, site);
            ForestFloor.AddLitter(foliarInput, cohort.Species, site);

            if (disturbanceType != null)
                Disturbed[site] = true;

            return;
        }

        //---------------------------------------------------------------------

        protected override void AgeCohorts(ActiveSite site,
                                           ushort years,
                                           int? successionTimestep)
        {
            GrowCohorts(site, years, successionTimestep.HasValue);
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Grows all cohorts at a site for a specified number of years.  The
        /// dead pools at the site also decompose for the given time period.
        /// </summary>
        public static void GrowCohorts(
                                       ActiveSite site,
                                       int years,
                                       bool isSuccessionTimestep)
        {
            //PlugIn.ModelCore.Log.WriteLine("years = {0}, successionTS = {1}.", years, successionTimestep.Value);

            for (int y = 1; y <= years; ++y)
            {
                if (PlugIn.ModelCore.CurrentTime > 0)
                    SpeciesData.GetAnnualData(PlugIn.ModelCore.CurrentTime + y - 1);

                SiteVars.ResetAnnualValues(site);
                CohortBiomass.SubYear = y - 1;
                if (y == 1)
                    SiteVars.Cohorts[site].Grow(site, (y == years && isSuccessionTimestep), true);
                else
                    SiteVars.Cohorts[site].Grow(site, (y == years && isSuccessionTimestep), false);
                
                double oldWood = SiteVars.WoodyDebris[site].Mass;
                SiteVars.WoodyDebris[site].Decompose();
                SiteVars.Litter[site].Decompose();
            }

        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Determines if there is sufficient light at a site for a species to
        /// germinate/resprout.
        /// This is a Delegate method to base succession.
        /// </summary>
        public bool SufficientLight(ISpecies species, ActiveSite site)
        {

            byte siteShade = PlugIn.ModelCore.GetSiteVar<byte>("Shade")[site];

            double lightProbability = 0.0;

            bool found = false;

            foreach (ISufficientLight lights in sufficientLight)
            {

                //PlugIn.ModelCore.Log.WriteLine("Sufficient Light:  ShadeClass={0}, Prob0={1}.", lights.ShadeClass, lights.ProbabilityLight0);
                if (lights.ShadeClass == SpeciesData.ShadeTolerance[species])
                {
                    if (siteShade == 0) lightProbability = lights.ProbabilityLight0;
                    if (siteShade == 1) lightProbability = lights.ProbabilityLight1;
                    if (siteShade == 2) lightProbability = lights.ProbabilityLight2;
                    if (siteShade == 3) lightProbability = lights.ProbabilityLight3;
                    if (siteShade == 4) lightProbability = lights.ProbabilityLight4;
                    if (siteShade == 5) lightProbability = lights.ProbabilityLight5;
                    found = true;
                }
            }

            if (!found) PlugIn.ModelCore.UI.WriteLine("Could not find sufficient light data for {0}.", species.Name);

            return PlugIn.ModelCore.GenerateUniform() < lightProbability;
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Add a new cohort to a site.
        /// This is a Delegate method to base succession.
        /// </summary>

        public void AddNewCohort(ISpecies species, ActiveSite site, string reproductionType, double propBiomass = 1.0)
        {
            SiteVars.Cohorts[site].AddNewCohort(species, 1, CohortBiomass.InitialBiomass(species, SiteVars.Cohorts[site], site), new System.Dynamic.ExpandoObject());
        }
        //---------------------------------------------------------------------

        /// <summary>
        /// Determines if a species can establish on a site.
        /// This is a Delegate method to base succession.
        /// </summary>
        public bool Establish(ISpecies species, ActiveSite site)
        {
            IEcoregion ecoregion = modelCore.Ecoregion[site];
            double establishProbability = SpeciesData.EstablishProbability[species,ecoregion];
            double modEstabProb = establishProbability * SpeciesData.EstablishModifier[species,ecoregion];

            return modelCore.GenerateUniform() < establishProbability;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Determines if there is a mature cohort at a site.  
        /// This is a Delegate method to base succession.
        /// </summary>
        public bool MaturePresent(ISpecies species, ActiveSite site)
        {
            return SiteVars.Cohorts[site].IsMaturePresent(species);
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Determines if a species can establish on a site.
        /// This is a Delegate method to base succession.
        /// </summary>
        public bool PlantingEstablish(ISpecies species, ActiveSite site)
        {
            IEcoregion ecoregion = modelCore.Ecoregion[site];
            double establishProbability = SpeciesData.EstablishProbability[species,ecoregion];

            return establishProbability > 0.0;
        }
        public override void InitializeSites(string initialCommunitiesText, string initialCommunitiesMap, ICore modelCore)
        {
            ModelCore.UI.WriteLine("   Loading initial communities from file \"{0}\" ...", initialCommunitiesText);
            Landis.Library.InitialCommunities.Universal.DatasetParser parser = new Landis.Library.InitialCommunities.Universal.DatasetParser(Timestep, ModelCore.Species, additionalCohortParameters, initialCommunitiesText);
            Landis.Library.InitialCommunities.Universal.IDataset communities = Landis.Data.Load<Landis.Library.InitialCommunities.Universal.IDataset>(initialCommunitiesText, parser);

            ModelCore.UI.WriteLine("   Reading initial communities map \"{0}\" ...", initialCommunitiesMap);
            IInputRaster<UIntPixel> map;
            map = ModelCore.OpenRaster<UIntPixel>(initialCommunitiesMap);
            using (map)
            {
                UIntPixel pixel = map.BufferPixel;
                foreach (Site site in ModelCore.Landscape.AllSites)
                {
                    map.ReadBufferPixel();
                    uint mapCode = pixel.MapCode.Value;
                    if (!site.IsActive)
                        continue;

                    //if (!modelCore.Ecoregion[site].Active)
                    //    continue;

                    //modelCore.Log.WriteLine("ecoregion = {0}.", modelCore.Ecoregion[site]);

                    ActiveSite activeSite = (ActiveSite)site;
                    initialCommunity = communities.Find(mapCode);
                    if (initialCommunity == null)
                    {
                        //ModelCore.UI.WriteLine("   Map Code {0} does not have an initial community", mapCode);
                        SiteVars.Cohorts[site] = new SiteCohorts();
                    }
                    else
                    {
                        InitializeSite(activeSite);
                    }
                }
            }
        }

        public override void AddCohortData()
        {
            // No new parameters to add
            return;
        }
    }
}
