//  Copyright 2005-2010 Portland State University, University of Wisconsin
//  Authors:  Robert M. Scheller, James B. Domingo

using Landis.Library.BiomassCohorts;
using Edu.Wisc.Forest.Flel.Util;

namespace Landis.Extension.Succession.Biomass.AgeOnlyDisturbances
{
    /// <summary>
    /// The event handlers when no biomass parameters have been provided by
    /// the user.  
    /// </summary>
    public static class NoParameters
    {
        public static void CohortDied(object         sender,
                                      DeathEventArgs eventArgs)
        {
            ThrowException();
        }

        //---------------------------------------------------------------------

        public static void SiteDisturbed(object               sender,
                                         DisturbanceEventArgs eventArgs)
        {
            ThrowException();
        }

        //---------------------------------------------------------------------

        private static void ThrowException()
        {
            string[] mesg = new string[] {
                "Error:  An age-only disturbance has occurred, but no biomass",
                "        parameters were provided for age-only disturbances."
            };
            throw new MultiLineException(new MultiLineText(mesg));
        }
    }
}
