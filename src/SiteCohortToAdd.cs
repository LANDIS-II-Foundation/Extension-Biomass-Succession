//  Authors:  Caren Dymond, Sarah Beukema

using Landis.Core;
using Landis.SpatialModeling;

namespace Landis.Extension.Succession.Biomass
{
    public struct SiteCohortToAdd
    {
        public ActiveSite site;

        public ISpecies species;

        public int newBiomass;

        public SiteCohortToAdd(ActiveSite site, ISpecies species, int newBiomass)
        {
            this.site = site;
            this.species = species;
            this.newBiomass = newBiomass;
        }
    }
}