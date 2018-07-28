using System;
using Landis.Library.Metadata;

namespace Landis.Extension.Succession.Biomass.utility
{
    class MetadataHandler
    {
        public static ExtensionMetadata Extension { get; set; }

        public static void InitializeMetadata(string summaryLogFileName)
        {
            ScenarioReplicationMetadata scenRep = new ScenarioReplicationMetadata()
            {
                RasterOutCellArea = PlugIn.ModelCore.CellArea,
                TimeMin = PlugIn.ModelCore.StartTime,
                TimeMax = PlugIn.ModelCore.EndTime,
            };

            Extension = new ExtensionMetadata(PlugIn.ModelCore)
            {
                Name = PlugIn.ExtensionName,
                TimeInterval = PlugIn.ModelCore.CurrentTime, //change this to PlugIn.TimeStep for other extensions
                ScenarioReplicationMetadata = scenRep
            };

            CreateDirectory(summaryLogFileName);
            PlugIn.summaryLog = new MetadataTable<SummaryLog>(summaryLogFileName);

            PlugIn.ModelCore.UI.WriteLine("   Generating summary table...");
            OutputMetadata tblOut_summary = new OutputMetadata()
            {
                Type = OutputType.Table,
                Name = "SummaryLog",
                FilePath = PlugIn.summaryLog.FilePath,
                Visualize = true,
            };
            tblOut_summary.RetriveFields(typeof(SummaryLog));
            Extension.OutputMetadatas.Add(tblOut_summary);

            //---------------------------------------
            MetadataProvider mp = new MetadataProvider(Extension);
            mp.WriteMetadataToXMLFile("Metadata", Extension.Name, Extension.Name);
        }
        public static void CreateDirectory(string path)
        {
            //Require.ArgumentNotNull(path);
            path = path.Trim(null);
            if (path.Length == 0)
                throw new ArgumentException("path is empty or just whitespace");

            string dir = System.IO.Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(dir))
            {
                Landis.Utilities.Directory.EnsureExists(dir);
            }

            //return new StreamWriter(path);
            return;
        }
    }
}