//  Authors:  Robert M. Scheller, Brian R. Miranda 

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Data;
using System.Text.RegularExpressions;

namespace Landis.Extension.Succession.Biomass
{
    //CSVParser derived from code posted at http://aspnetcafe.com/post/CSV-to--DataTable-Parser.aspx
    //Modified for LANDIS-II by Brendan C. Ward, 4/15/2008
    public class CSVParser
    {
        string delimiter = ",";
        string quotes = "\"";
        System.Text.RegularExpressions.Regex CSVregEx = new System.Text.RegularExpressions.Regex("(\"([^\"]*|\"{2})*\"(,|$))|\"[^\"]*\"(,|$)|[^,]+(,|$)|(,)");
        StreamReader reader;

        public CSVParser(){ }

        protected string[] BreakCSV(string source)
        {//Use regex to break CSV at delimiters
            MatchCollection matches = CSVregEx.Matches(source);

            string[] res = new string[matches.Count];
            int i = 0;
            foreach (Match m in matches)
            {
                res[i] = m.Groups[0].Value.TrimEnd(delimiter[0]).Trim(quotes[0]);
                i++;
            }
            return res;
        }

        public DataTable ParseToDataTable(string path)
        {
            reader = new StreamReader(path);

            DataTable result = new DataTable();
            result.TableName = "CSV";
            int DeduceRowCount = 5;//rows of data to read for deducing data type
            string line = "";
            int read_row_count=0;

            //header
            string header = reader.ReadLine();
            string[] column_names = BreakCSV(header);

            List<string> DeduceRows = new List<string>();
            List<int> ColTypes = new List<int>();
            for (int i = 0; i < column_names.Length; i++)
                ColTypes.Add(0);
            //Deduction codes:
            //Not Set=0
            //String=1
            //Int32=2
            //Double=3
            //Bool=4

            //need to read the first DeduceRowCount lines and deduce types of each column
            while (!reader.EndOfStream && read_row_count<DeduceRowCount)
            {
                line = reader.ReadLine();
                read_row_count++;
                DeduceRows.Add(line);
                string[] data = BreakCSV(line);
                int col_type = 0;
                int test_int=0;
                double test_double=0.0;
                bool test_bool = false;
                for (int i = 0; i < data.Length;i++)
                {
                    string entry = data[i];
                    if (entry.Trim().Length < 1)  // If there are blank fields (no column heading, no data), skip them.
                        continue;

                    col_type = 1;
                    try
                    {
                        test_int = Convert.ToInt32(entry);
                        col_type = 2;
                    }
                    catch
                    {//Conversion to Int32 failed, could be Double or string
                        try
                        {
                            test_double = Convert.ToDouble(entry);
                            col_type = 3;
                        }
                        catch
                        {//Conversion to Double failed, could be boolean or string at this point
                            try
                            {
                                test_bool = Convert.ToBoolean(entry);
                                col_type = 4;
                            }
                            catch
                            {//Conversion to Boolean failed, can only be a string at this point
                            }
                        }
                    }
                    ColTypes[i] = Math.Max(col_type, ColTypes[i]);//If we currently have int, but previously had Double, then make sure we remember that
                }
            }

            //Add the column to the DataTable, with its associated type
            for (int i=0;i<ColTypes.Count;i++)
            {
                DataColumn column;
                switch(ColTypes[i])
                {
                    case 2:
                        column = new DataColumn(column_names[i], typeof(Int32));
                        break;
                    case 3:
                        column = new DataColumn(column_names[i], typeof(Double));
                        break;
                    case 4:
                        column = new DataColumn(column_names[i], typeof(Boolean));
                        break;
                    default:
                        column = new DataColumn(column_names[i], typeof(String));
                        break;
                }
                column.ColumnMapping = MappingType.Attribute;
                result.Columns.Add(column);
            }

            //Now add the DeduceRows to DataTable before moving on
            foreach(string deduced_line in DeduceRows)
            {
                string[] data = BreakCSV(deduced_line);
                int i = 0;
                DataRow dr = result.NewRow();
                foreach (string d in data)
                {
                    dr[i++] = d;
                }
                result.Rows.Add(dr);        
            }
            //Now read the remainder of the data           
            while (!reader.EndOfStream)
            {
                line = reader.ReadLine();
                string[] data = BreakCSV(line);
                int i = 0;
                DataRow dr = result.NewRow();
                foreach (string d in data)
                {
                    dr[i++] = d;
                }
                result.Rows.Add(dr);
            }

            reader.Close();
            return result;
        }
    }
}
