//  Authors:  Robert M. Scheller, James B. Domingo

using Landis.Utilities;
using System.Collections.Generic;

namespace Landis.Extension.Succession.Biomass
{
    /// <summary>
    /// Methods for working with the template for map filenames.
    /// </summary>
    public static class MapNames
    {
        public const string TimestepVar = "timestep";
        public const string DirectoryVar = "slash";

        private static IDictionary<string, bool> knownVars;
        private static IDictionary<string, string> varValues;

        //---------------------------------------------------------------------

        static MapNames()
        {
            knownVars = new Dictionary<string, bool>();
            knownVars[TimestepVar] = true;
            knownVars[DirectoryVar] = true;

            varValues = new Dictionary<string, string>();
        }

        //---------------------------------------------------------------------

        public static void CheckTemplateVars(string template)
        {
            OutputPath.CheckTemplateVars(template, knownVars);
        }

        //---------------------------------------------------------------------

        public static string ReplaceTemplateVars(string template, char slash, int timestep)
        {
            varValues[TimestepVar] = timestep.ToString();
            varValues[DirectoryVar] = slash.ToString();
            return OutputPath.ReplaceTemplateVars(template, varValues);
        }
    }
}
