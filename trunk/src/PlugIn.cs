//  Copyright 2005-2010 Portland State University, University of Wisconsin
//  Authors:  Robert M. Scheller

using Landis.Core;
using Landis.SpatialModeling;
using Landis.Library.Succession;
using Landis.Library.InitialCommunities;
using Landis.Library.BiomassCohorts;
using System.Collections.Generic;

namespace Landis.Extension.Succession.Biomass
{
    public class PlugIn
        : Landis.Library.Succession.ExtensionBase
    {
        public static readonly string ExtensionName = "Biomass Succession";
        private static ICore modelCore;
        private IInputParameters parameters;

        private List<ISufficientLight> sufficientLight;
        public static bool CalibrateMode;
        public static double CurrentYearSiteMortality;
        private static int time;

        //---------------------------------------------------------------------

        public PlugIn()
            : base(ExtensionName)
        {
        }

        //---------------------------------------------------------------------

        public override void LoadParameters(string dataFile, ICore mCore)
        {
            modelCore = mCore;
            SiteVars.Initialize();
            InputParametersParser parser = new InputParametersParser();
            parameters = Landis.Data.Load<IInputParameters>(dataFile, parser);
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

            Timestep = parameters.Timestep;
            time = Timestep;
            CalibrateMode = parameters.CalibrateMode;
            CohortBiomass.SpinupMortalityFraction = parameters.SpinupMortalityFraction;

            sufficientLight = parameters.LightClassProbabilities;

            SpeciesData.Initialize(parameters);
            EcoregionData.Initialize(parameters);
            DynamicInputs.Initialize(parameters.DynamicInputFile, false);
            SpeciesData.ChangeDynamicParameters(0);  // Year 0
            Outputs.Initialize(parameters);
            
            //  Cohorts must be created before the base class is initialized
            //  because the base class' reproduction module uses the core's
            //  SuccessionCohorts property in its Initialization method.
            Landis.Library.BiomassCohorts.Cohorts.Initialize(Timestep, new CohortBiomass());

            // Initialize Reproduction routines:
            Reproduction.SufficientResources = SufficientLight;
            Reproduction.Establish = Establish;
            Reproduction.AddNewCohort = AddNewCohort;
            Reproduction.MaturePresent = MaturePresent;
            Reproduction.PlantingEstablish = PlantingEstablish;
            base.Initialize(modelCore, parameters.SeedAlgorithm); 

            InitialBiomass.Initialize(Timestep);

            Cohort.DeathEvent += CohortDied;
            AgeOnlyDisturbances.Module.Initialize(parameters.AgeOnlyDisturbanceParms);

            InitializeSites(parameters.InitialCommunities, parameters.InitialCommunitiesMap, modelCore);
        }


        //---------------------------------------------------------------------

        protected override void InitializeSite(ActiveSite site,
                                               ICommunity initialCommunity)
        {
            InitialBiomass initialBiomass = InitialBiomass.Compute(site, initialCommunity);
            SiteVars.Cohorts[site] = InitialBiomass.Clone(initialBiomass.Cohorts); //.Clone();
            SiteVars.WoodyDebris[site] = initialBiomass.DeadWoodyPool.Clone();
            SiteVars.Litter[site] = initialBiomass.DeadNonWoodyPool.Clone();
        }

        //---------------------------------------------------------------------

        public override void Run()
        {
            if(PlugIn.ModelCore.CurrentTime == Timestep)
                Outputs.WriteLogFile(0);

            if(PlugIn.ModelCore.CurrentTime > 0 && SiteVars.CapacityReduction == null)
                SiteVars.CapacityReduction   = PlugIn.ModelCore.GetSiteVar<double>("Harvest.CapacityReduction");

            base.Run();

            Outputs.WriteLogFile(PlugIn.ModelCore.CurrentTime);

            // Reset establishment modifier to 1.0 after each time step
            foreach (ISpecies species in PlugIn.ModelCore.Species)
            {
                foreach (IEcoregion ecoregion in PlugIn.ModelCore.Ecoregions)
                {
                    if (!ecoregion.Active)
                            continue;
                    SpeciesData.EstablishModifier[species,ecoregion] = 1.0;
                }
            }
        }


        //---------------------------------------------------------------------
        // Revised 10/5/09 - BRM

        /*public override byte ComputeShade(ActiveSite site)
        {
            // Use correlation from Scheller and Mladenoff (Figure 4b)
            // to assign a shade class depending on percent transmittance.


            SiteVars.PercentShade[site] = 1.0 - SiteVars.LightTrans[site];

            double percentSun = System.Math.Max(1.0 - SiteVars.PercentShade[site], 0.0);
            percentSun = System.Math.Min(1.0, percentSun);

            percentSun = percentSun * 100.0;

            byte shadeClass = 0;

            if (percentSun < pctSun1) shadeClass = 1;
            if (percentSun < pctSun2) shadeClass = 2;
            if (percentSun < pctSun3) shadeClass = 3;
            if (percentSun < pctSun4) shadeClass = 4;
            if (percentSun < pctSun5) shadeClass = 5;

            return shadeClass;

        }*/
        public override byte ComputeShade(ActiveSite site)
        {
            //return LivingBiomass.ComputeShade(site);
            IEcoregion ecoregion = ModelCore.Ecoregion[site];
            double B_ACT = 0.0;

            if (SiteVars.Cohorts[site] != null)
            {
                foreach (ISpeciesCohorts sppCohorts in SiteVars.Cohorts[site])
                    foreach (ICohort cohort in sppCohorts)
                        if (cohort.Age > 5)
                            B_ACT += cohort.Biomass;
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

        public void CohortDied(object sender,
                               DeathEventArgs eventArgs)
        {
            ExtensionType disturbanceType = eventArgs.DisturbanceType;
            if (disturbanceType != null)
            {
                ActiveSite site = eventArgs.Site;
                Disturbed[site] = true;
                if (disturbanceType.IsMemberOf("disturbance:fire"))
                    Reproduction.CheckForPostFireRegen(eventArgs.Cohort, site);
                else
                    Reproduction.CheckForResprouting(eventArgs.Cohort, site);
            }
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
                    SpeciesData.ChangeDynamicParameters(PlugIn.ModelCore.CurrentTime + y - 1);

                SiteVars.ResetAnnualValues(site);
                CohortBiomass.SubYear = y - 1;

                SiteVars.Cohorts[site].Grow(site, (y == years && isSuccessionTimestep));
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
                if (lights.ShadeClass == species.ShadeTolerance)
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

        public void AddNewCohort(ISpecies species, ActiveSite site)
        {
            SiteVars.Cohorts[site].AddNewCohort(species, 1, CohortBiomass.InitialBiomass(species, SiteVars.Cohorts[site], site));
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

            return modelCore.GenerateUniform() < modEstabProb;
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

    }
}
