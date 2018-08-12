using AgeOnlyCohorts = Landis.Library.AgeOnlyCohorts;
using BiomassCohorts = Landis.Library.BiomassCohorts;
using Landis.SpatialModeling;

namespace Landis.Extension.Succession.Biomass
{
    /// <summary>
    /// Wraps a biomass-cohorts site variable and provides access to it as a
    /// site variable of base cohorts.
    /// </summary>
    public class BaseCohortsSiteVar
        : ISiteVar<AgeOnlyCohorts.ISiteCohorts>
    {
        private ISiteVar<BiomassCohorts.ISiteCohorts> biomassCohortSiteVar;

        public BaseCohortsSiteVar(ISiteVar<BiomassCohorts.ISiteCohorts> siteVar)
        {
            biomassCohortSiteVar = siteVar;
        }

        #region ISiteVariable members
        System.Type ISiteVariable.DataType
        {
            get
            {
                return typeof(AgeOnlyCohorts.ISiteCohorts);
            }
        }

        InactiveSiteMode ISiteVariable.Mode
        {
            get
            {
                return biomassCohortSiteVar.Mode;
            }
        }

        ILandscape ISiteVariable.Landscape
        {
            get
            {
                return biomassCohortSiteVar.Landscape;
            }
        }
        #endregion

        #region ISiteVar<BaseCohorts.ISiteCohorts> members
        // Extensions other than succession have no need to assign the whole
        // site-cohorts object at any site.

        AgeOnlyCohorts.ISiteCohorts ISiteVar<AgeOnlyCohorts.ISiteCohorts>.this[Site site]
        {
            get
            {
                return (AgeOnlyCohorts.ISiteCohorts) biomassCohortSiteVar[site];
                //return biomassCohortSiteVar[site]; 
            }
            set
            {
                throw new System.InvalidOperationException("Operation restricted to succession extension");
            }
        }

        AgeOnlyCohorts.ISiteCohorts ISiteVar<AgeOnlyCohorts.ISiteCohorts>.ActiveSiteValues
        {
            set
            {
                throw new System.InvalidOperationException("Operation restricted to succession extension");
            }
        }

        AgeOnlyCohorts.ISiteCohorts ISiteVar<AgeOnlyCohorts.ISiteCohorts>.InactiveSiteValues
        {
            set
            {
                throw new System.InvalidOperationException("Operation restricted to succession extension");
            }
        }

        AgeOnlyCohorts.ISiteCohorts ISiteVar<AgeOnlyCohorts.ISiteCohorts>.SiteValues
        {
            set
            {
                throw new System.InvalidOperationException("Operation restricted to succession extension");
            }
        }
        #endregion
    }
}
